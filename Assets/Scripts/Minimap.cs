using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    // determines DISTANCE scale
    public const float SCALE = 1.5f;

    [SerializeField]
    GameObject planetMarkerPrefab;
    [SerializeField]
    GameObject playerMarkerPrefab;

    // store player object for reference
    private GameObject player;
    // player marker - changed on frame update
    private GameObject playerMarker;
    // global level object
    private Level level;

    // Start is called before the first frame update
    void Start()
    {
        playerMarker = Instantiate(playerMarkerPrefab);
        playerMarker.transform.SetParent(transform, false);

        player = GameObject.FindWithTag("Player");
        if (!(player is null))
        {
            // playerMarker can be set to player's actual position
            Vector2 pos = player.transform.position.normalized;
            playerMarker.GetComponent<RectTransform>().anchoredPosition = pos * SCALE * (200.0f / level.Radius);
        }
    }

    // assigns the positions of the planets on the map
    public void Initialize(List<PlanetData> planets)
    {
        level = GameObject.FindWithTag("Level").GetComponent<Level>();
        foreach (PlanetData planet in planets)
        {
            GameObject planetMarker = Instantiate(planetMarkerPrefab);
            planetMarker.transform.SetParent(transform, false);
            planetMarker.GetComponent<RectTransform>().anchoredPosition = planet.Pos * SCALE * (200.0f / level.Radius);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player is null)
        {
            player = GameObject.FindWithTag("Player");
        }

        if (player is null)
            return; // don't bother trying to update player pos
        else
            playerMarker.GetComponent<RectTransform>().anchoredPosition = player.transform.position * SCALE * (200.0f / level.Radius);
    }
}
