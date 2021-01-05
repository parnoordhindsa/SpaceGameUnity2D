using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public struct WormholeData : SpaceData
{
    public Vector2 Pos { get; set; }
    public float Radius { get; set; }
}

// where a planet is, what's its radius
public struct PlanetData : SpaceData
{
    public Vector2 Pos { get; set; }
    public float Radius { get; set; }
    public bool Wormhole { get; set; } // whether or not this planet should spawn a wormhole
}

public struct AsteroidData : SpaceData
{
    public Vector2 Pos { get; set; }
    public float Radius { get; set; }
    public Vector2 Direction { get; set; }
}

// data about one specific enemy
public struct EnemyData : SpaceData
{
    public Vector2 Pos { get; set; }
    public float Radius { get; set; }
    public int Level { get; set; }
}

// data about a cluster of generated enemies
public struct EnemyGroupData : SpaceData
{
    public Vector2 Pos { get; set; }
    public float Radius { get; set; }
}

// locations of level features
public struct LevelData
{
    public float radius;
    public WormholeData wormhole;
    public List<PlanetData> planets;
    public List<AsteroidData> borderAsteroids;
    public List<AsteroidData> fieldAsteroids;
    public List<EnemyData> enemies;
    public Vector2 playerPos;
    public PositionData positionData;
}

// mean radius of planets, and how far from the mean radius may be
public struct PlanetParams
{
    public float meanRadius;
    public float varRadius;
}

// parameters concerning the generation of all planets, not just one
public struct PlanetsParams
{
    // number of planets desired
    public int nPlanets;
    // how far planets should be from each other
    public float planetExclusionRadius;
    // how far planets should be from wormholes
    public float wormholeExclusionRadius;
}

public struct BorderAsteroidsParams
{
    // initial asteroid size
    public float baseRadius;
    // size increase step size
    public float deltaRadius;
    // layers of border asteroids
    public int layers;
}

public struct AsteroidFieldsParams
{
    // offset used for perlin noise
    public Vector2 perlinBase;
    // scale used for perlin noise
    public float perlinScale;
    // used to determine how many points are sampled from perlin noise
    public float perlinGrain;
    // noise threshhold for considering space to be asteroid field
    public float threshhold;
}

public struct FieldAsteroidParams
{
    // offset used for perlin noise
    public Vector2 perlinBase;
    // scale used for perlin noise
    public float perlinScale;
    // mean distance between asteroids (center, not borders)
    public float meanDistance;
    // maximum deviance from mean distance allowed
    public float varDistance;
    // mean radius of asteroids in fields
    public float meanRadius;
    // maximum deviance from mean radius allowed
    public float varRadius;
}

public struct EnemiesParams
{
    // number of locations to spawn a group of enemies at
    public int enemySpots;
    // number of enemies to spawn per spawnpoint
    public int nEnemies;
}

public class LevelGenerator
{
    public const float DEFAULT_RADIUS = 500.0f;
    public const float WORMHOLE_RADIUS = 0.7f * 2.9f;
    public const int PLANET_CANDIDATES = 3;
    public const float CHUNK_SIZE = 20.0f;
    public const float ENEMY_RADIUS = 0.8f; // rounded-up approximation

    public const int NO_PLANET_SPACE = 1;

    // levelData to generate
    private LevelData levelData;

    // determines the features of individually generated planets
    private PlanetParams planetParams;

    // determines general parameters for generating planets
    private PlanetsParams planetsParams;

    // determines generation pattern for border asteroids
    private BorderAsteroidsParams borderAsteroidsParams;

    // determines how asteroid field regions are decided
    private AsteroidFieldsParams asteroidFieldsParams;

    // determines characteristics for individual asteroids in fields
    private FieldAsteroidParams fieldAsteroidParams;

    // determines how groups of enemies spawn
    private EnemiesParams enemiesParams;

    // fixed random object
    private System.Random random;

    // levelData.radius must be assigned at this point!
    // postcond: levelData.wormholes have been assigned
    private int AddWormholes()
    {
        // the entry wormhole is just somwhere along the edge of the map, at a random angle
        float theta = (float)random.NextDouble() * 2 * Mathf.PI;

        levelData.wormhole.Pos = (levelData.radius - WORMHOLE_RADIUS) * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        levelData.wormhole.Radius = WORMHOLE_RADIUS;

        /*
        // the second wormhole is also chosen by selecting a random angle, but it must be at least 90 degrees away
        float baseTheta = theta - Mathf.PI;
        theta = Mathf.Lerp(baseTheta - Mathf.PI / 2, baseTheta + Mathf.PI / 2, (float)random.NextDouble());
        if (theta < 0.0f) theta += 2 * Mathf.PI;

        levelData.wormholes[1].Pos = (levelData.radius - WORMHOLE_RADIUS) * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        levelData.wormholes[1].Radius = WORMHOLE_RADIUS; */

        levelData.playerPos = (levelData.radius - 2 * WORMHOLE_RADIUS) * levelData.wormhole.Pos.normalized;
        levelData.positionData.Add(levelData.wormhole);
        
        return 0;
    }

    // penalize planets for being too close to the wormhole(s) (both of them) and for being within level radius
    // wormholes should already be initialized; see other overload for what return value is/means
    private float PlanetLoss(SpaceData x)
    {
        if (x.Pos.magnitude + x.Radius >= levelData.radius)
            return 0.0f;
        float d = (x.Pos - levelData.wormhole.Pos).magnitude - x.Radius - WORMHOLE_RADIUS;
        d = Mathf.Clamp(d / planetsParams.wormholeExclusionRadius, 0.0f, 1.0f);
        return d;
    }

    // penalizes planets for being too close together (on range [0,1], where lower means its less likely)
    // a penalty of 1 means its completely fine; a penalty of 0 probably means the planets overlap
    private float PlanetsLoss(SpaceData a, SpaceData b)
    {
        float d = (a.Pos - b.Pos).magnitude - a.Radius - b.Radius;
        d = Mathf.Clamp(d / planetsParams.planetExclusionRadius, 0.0f, 1.0f);
        return d;
    }

    // returns whether spatial body collides with wormholes/level border
    private bool ObjectCollide(SpaceData x)
    {
        if (x.Pos.magnitude + x.Radius >= levelData.radius)
            return true;
        if ((x.Pos - levelData.wormhole.Pos).magnitude <= x.Radius + WORMHOLE_RADIUS)
            return true;
        return false;
    }

    private bool ObjectsCollide(SpaceData a, SpaceData b)
    {
        if ((a.Pos - b.Pos).magnitude <= a.Radius + b.Radius)
            return true;
        else return false;
    }

    // calculates the distance (edge-to-edge) from x to its nearest neighbour/level edge
    // if x is colliding with anything (distance <= 0), this function short-circuits
    // assumes that x has neighbours
    // also returns the nearest neighbour that it found, or null for level boundary
    // VERY EXPENSIVE
    private Tuple<float, SpaceData> NearestNeighbour(SpaceData x)
    {
        float distance = levelData.radius - x.Pos.magnitude - x.Radius;
        if (distance <= 0.0f) return new Tuple<float, SpaceData>(distance, null);
        // we make a WormholeData since its an arbitrary struct that implements SpaceData
        // setting Radius = distance ensures that if there is a neighbour closer than distance it is returned
        SpaceData @object = null;
        WormholeData data = new WormholeData { Pos = x.Pos, Radius = distance };
        List<SpaceData> neighbours = levelData.positionData.GetNeighbours(data);
        foreach (SpaceData spaceData in neighbours)
        {
            float d = (spaceData.Pos - x.Pos).magnitude - x.Radius - spaceData.Radius;
            if (d <= 0.0f)
                return new Tuple<float, SpaceData>(d, spaceData);
            else if (d < distance)
            {
                distance = d;
                @object = spaceData;
            }
        }
        return new Tuple<float, SpaceData>(distance, @object);
    }

    // must come after AddWormholes succeeds!
    // postcond: levelData.planets is filled up
    private int AddPlanets()
    {
        for (int i = 0; i < planetsParams.nPlanets; ++i)
        {
            float radius = planetParams.meanRadius + Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble()) * planetParams.varRadius;
            // candidates: possible locations to put a new planet
            PlanetData[] candidates = new PlanetData[PLANET_CANDIDATES];
            // scores: cumulative sum of planet losses, used to decide which spot to place a planet
            float[] scores = new float[PLANET_CANDIDATES];
            for (int j = 0; j < PLANET_CANDIDATES; ++j)
            {
                candidates[j] = new PlanetData
                {
                    Pos = new Vector2
                    {
                        x = Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble()) * levelData.radius,
                        y = Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble()) * levelData.radius
                    },
                    Radius = radius,
                    Wormhole = false
                };
                float score = PlanetLoss(candidates[j]);
                foreach (PlanetData other in levelData.planets)
                    score *= PlanetsLoss(candidates[j], other);
                if (j == 0)
                    scores[j] = score;
                else
                    scores[j] = scores[j - 1] + score;
            }

            float max = scores[PLANET_CANDIDATES - 1]; // last score is sum of all scores
            if (max == 0.0f)
            {
                --i; // just retry adding this planet
                continue;
            }
            float rand = max * (float)random.NextDouble();
            int choice = 0;
            while (rand > scores[choice])
            {
                ++choice;
                if (choice == PLANET_CANDIDATES - 1) break;
            }
            levelData.planets.Add(candidates[choice]);
            levelData.positionData.Add(candidates[choice]);
        }

        // choose the planet furthest from wormhole to be the exit planet
        float maxDistance = 0.0f;
        int maxIndex = -1;
        for (int i = 0; i < planetsParams.nPlanets; ++i)
        {
            float distance = (levelData.planets[i].Pos - levelData.wormhole.Pos).magnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }
        if (maxDistance == 0.0f) // should never happen unless radius is crazy small
        {
            throw new Exception("No planets were generated; level is inescapable");
        }
        PlanetData wormholePlanet = levelData.planets[maxIndex];
        wormholePlanet.Wormhole = true;
        levelData.planets[maxIndex] = wormholePlanet;

        return 0;
    }

    // add rings of asteroids around the level
    public int AddAsteroidBorder()
    {
        float distance = levelData.radius + borderAsteroidsParams.baseRadius + 10.0f;
        float radius = borderAsteroidsParams.baseRadius;
        float epsilon = 1e-3f;

        for (int i = 0; i < borderAsteroidsParams.layers; ++i)
        {
            float delta = (2 * radius) * 1.5f; // approximate distance between two asteroids
            float perimeter = 2 * Mathf.PI * distance;
            int nAsteroids = Mathf.FloorToInt(perimeter / delta);
            float deltaTheta = 2 * Mathf.PI / nAsteroids;

            float theta = 0.0f;
            while (Mathf.Abs(theta - 2 * Mathf.PI) >= epsilon)
            {
                AsteroidData asteroidData = new AsteroidData { Direction = Vector2.zero, Radius = radius, Pos = new Vector2(
                    Mathf.Cos(theta) * distance, Mathf.Sin(theta) * distance) };
                levelData.borderAsteroids.Add(asteroidData);
                theta += deltaTheta;
            }

            distance += (radius * 2 + borderAsteroidsParams.deltaRadius) * 2.0f;
            radius += borderAsteroidsParams.deltaRadius;
        }

        return 0;
    }

    // add fields of asteroids
    public int AddAsteroidFields()
    {
        Vector2 pos = new Vector2 { x = -levelData.radius };
        // first we add a grid of asteroids based on perlin noise values i.e. if noise is above a threshhold, place an asteroid
        while (pos.x <= levelData.radius)
        {
            pos.y = -levelData.radius;
            while (pos.y <= levelData.radius)
            {
                Vector2 perlinPos = (pos - asteroidFieldsParams.perlinBase) * asteroidFieldsParams.perlinScale;
                if (Mathf.PerlinNoise(perlinPos.x, perlinPos.y) >= asteroidFieldsParams.threshhold)
                {
                    AsteroidData asteroidData = new AsteroidData
                    {
                        Direction = Vector2.zero,
                        Pos = pos,
                        Radius = Mathf.Lerp(fieldAsteroidParams.meanRadius - fieldAsteroidParams.varRadius,
                            fieldAsteroidParams.meanRadius - fieldAsteroidParams.varRadius,
                            (float)random.NextDouble())
                    };
                    bool invalid = ObjectCollide(asteroidData);
                    if (!invalid)
                    {
                        List<SpaceData> neighbours = levelData.positionData.GetNeighbours(asteroidData);
                        foreach (SpaceData spaceData in neighbours)
                        {
                            invalid = ObjectsCollide(asteroidData, spaceData);
                            if (invalid) break;
                        }
                    }
                    if (!invalid)
                    {
                        levelData.fieldAsteroids.Add(asteroidData);
                        levelData.positionData.Add(asteroidData);
                    }
                }
                pos.y += asteroidFieldsParams.perlinGrain;
            }
            pos.x += asteroidFieldsParams.perlinGrain;
        }

        // then, each asteroid that has been placed gets to try to make a new neighbour asteroid
        // asteroids that are added in this manner also get to do so, allowing for strings of asteroids, which looks nice
        int i = 0;
        while (i < levelData.fieldAsteroids.Count)
        {
            AsteroidData asteroid = levelData.fieldAsteroids[i];
            float dir = 2 * Mathf.PI * (float)random.NextDouble();
            Vector2 perlinPos = (asteroid.Pos - fieldAsteroidParams.perlinBase) * fieldAsteroidParams.perlinScale;
            float len = Mathf.Lerp(fieldAsteroidParams.meanDistance - fieldAsteroidParams.varDistance,
                fieldAsteroidParams.meanDistance + fieldAsteroidParams.varDistance, Mathf.PerlinNoise(perlinPos.x, perlinPos.y));
            AsteroidData asteroidData = new AsteroidData
            {
                Direction = Vector2.zero,
                Radius = Mathf.Lerp(fieldAsteroidParams.meanRadius - fieldAsteroidParams.varRadius,
                    fieldAsteroidParams.meanRadius + fieldAsteroidParams.varRadius, (float)random.NextDouble()),
            };
            asteroidData.Pos = asteroid.Pos + (len + asteroid.Radius + asteroidData.Radius) * new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));

            perlinPos = (asteroidData.Pos - asteroidFieldsParams.perlinBase) * asteroidFieldsParams.perlinScale;
            bool invalid = Mathf.PerlinNoise(perlinPos.x, perlinPos.y) < asteroidFieldsParams.threshhold
                || ObjectCollide(asteroidData);
            if (!invalid)
            {
                List<SpaceData> neighbours = levelData.positionData.GetNeighbours(asteroidData);
                foreach (SpaceData spaceData in neighbours)
                {
                    invalid = ObjectsCollide(asteroidData, spaceData);
                    if (invalid) break;
                }
            }
            if (!invalid)
            {
                levelData.fieldAsteroids.Add(asteroidData);
                levelData.positionData.Add(asteroidData);
            }
            ++i; // incremented regardless of whether asteroid found a new neighbour
        }

        return 0;
    }

    // determines the radius of a group of n enemies
    // enemies spawn in hexagonal groups
    private float EnemyGroupRadius(int n)
    {
        float r = ENEMY_RADIUS;
        int i = n - 1;
        int c = 1;
        while (i > 0)
        {
            i -= c * 6;
            r += 2 * ENEMY_RADIUS;
            ++c;
        }
        return r;
    }

    // generates the positions of enemies in a group of n enemies
    // positions are given as offsets from the middle point
    private List<Vector2> EnemyGroupPositions(int n)
    {
        List<Vector2> positions = new List<Vector2>();
        // center enemy
        --n;
        positions.Add(Vector2.zero);
        // and now the rest
        int shell = 1;
        int shellCount = shell * 6;
        int off = 0;
        while (n > 0)
        {
            float d = 2 * ENEMY_RADIUS * shell;
            float lowerTheta = (off / shell) * Mathf.PI / 3;
            float upperTheta = (off / shell + 1) * Mathf.PI / 3;

            Vector2 lowerPos = new Vector2 { x = d * Mathf.Cos(lowerTheta), y = d * Mathf.Sin(lowerTheta) };
            Vector2 upperPos = new Vector2 { x = d * Mathf.Cos(upperTheta), y = d * Mathf.Sin(upperTheta) };
            Vector2 pos = new Vector2();
            pos.x = Mathf.Lerp(lowerPos.x, upperPos.x, (off % shell) / (float)shell);
            pos.y = Mathf.Lerp(lowerPos.y, upperPos.y, (off % shell) / (float)shell);
            positions.Add(pos);

            ++off;
            if (off == shellCount)
            {
                ++shell;
                shellCount += 6;
                off = 0;
            }
            --n;
        }
        return positions;
    }

    // add groups of enemies
    public int AddEnemyGroups()
    {
        EnemyGroupData[] spawnpoints = new EnemyGroupData[enemiesParams.enemySpots];
        float radius = EnemyGroupRadius(enemiesParams.nEnemies);
        int i = 0;
        while (i < enemiesParams.enemySpots)
        {
            EnemyGroupData groupData = new EnemyGroupData
            {
                Pos = new Vector2
                {
                    x = Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble()) * levelData.radius,
                    y = Mathf.Lerp(-1.0f, 1.0f, (float)random.NextDouble()) * levelData.radius
                },
                Radius = radius
            };
            if (ObjectCollide(groupData)) continue;
            List<SpaceData> neighbours = levelData.positionData.GetNeighbours(groupData);
            foreach (SpaceData spaceData in neighbours)
            {
                if (ObjectsCollide(spaceData, groupData))
                    continue;
            }
            spawnpoints[i] = groupData;
            levelData.positionData.Add(groupData);
            ++i;
        }

        // it's only really important that the enemies spawn within the expected radius
        // we make them spawn in a hexagon for simplicity
        List<Vector2> positions = EnemyGroupPositions(enemiesParams.nEnemies);
        foreach (EnemyGroupData enemyGroupData in spawnpoints)
        {
            for (int j = 0; j < enemiesParams.nEnemies; ++j)
            {
                int level = (enemiesParams.nEnemies - j - 1) / 3;
                EnemyData enemyData = new EnemyData { Pos = enemyGroupData.Pos + positions[j], Radius = ENEMY_RADIUS, Level = level };
                levelData.enemies.Add(enemyData);
                // don't add enemy data to positionData, since data of group is already there
            }
        }
        return 0;
    }

    // initializes a generator with default generation values
    public LevelGenerator(float r = DEFAULT_RADIUS, int level = 0)
    {
        levelData = new LevelData { radius = r, planets = new List<PlanetData>(), wormhole = new WormholeData { Pos = new Vector2() }, playerPos = new Vector2(),
            positionData = new PositionData(CHUNK_SIZE), borderAsteroids = new List<AsteroidData>(), fieldAsteroids = new List<AsteroidData>(),
            enemies = new List<EnemyData>() };
        // all of this will need fine-tuning
        planetParams = new PlanetParams {
            meanRadius = 10.0f,
            varRadius = 1.0f
        };
        planetsParams = new PlanetsParams {
            nPlanets = (int)Math.Floor(Math.PI * r * r / 20000),
            planetExclusionRadius = 20.0f,
            wormholeExclusionRadius = 30.0f
        };
        borderAsteroidsParams = new BorderAsteroidsParams {
            baseRadius = 0.75f,
            deltaRadius = 0.25f,
            layers = 5
        };

        random = new System.Random(DateTime.Now.Second * 1000 + DateTime.Now.Millisecond);

        asteroidFieldsParams = new AsteroidFieldsParams
        {
            perlinBase = new Vector2(1000.0f * (float)random.NextDouble(), 1000.0f * (float)random.NextDouble()),
            perlinGrain = 10.0f,
            perlinScale = 0.025f,
            threshhold = 0.6f
        };
        fieldAsteroidParams = new FieldAsteroidParams
        {
            perlinBase = new Vector2(1000.0f * (float)random.NextDouble(), 1000.0f * (float)random.NextDouble()),
            perlinScale = 0.1f,
            meanRadius = 0.7f,
            varRadius = 0.4f,
            meanDistance = 2.0f,
            varDistance = 0.6f
        };
        enemiesParams = new EnemiesParams
        {
            enemySpots = Mathf.CeilToInt(Mathf.PI * levelData.radius * levelData.radius / 12000),
            nEnemies = 2 + level
        };
    }

    // this is supposed to generate a level in the background but I don't know how C# works
    public async Task<int> Generate()
    {
        int err;
        Task<int> task;

        task = Task.Run(AddWormholes);
        await task;
        err = task.Result;
        if (err != 0)
            Debug.LogError(err);

        task = Task.Run(AddPlanets);
        await task;
        err = task.Result;
        if (err != 0)
            Debug.LogError(err);

        task = Task.Run(AddAsteroidBorder);
        await task;
        err = task.Result;
        if (err != 0)
            Debug.LogError(err);

        task = Task.Run(AddAsteroidFields);
        await task;
        err = task.Result;
        if (err != 0)
            Debug.LogError(err);

        task = Task.Run(AddEnemyGroups);
        await task;
        err = task.Result;
        if (err != 0)
            Debug.LogError(err);

        return 0;
    }

    // Generate should finish before this is called
    public LevelData GetLevelData()
    {
        return levelData;
    }
}
