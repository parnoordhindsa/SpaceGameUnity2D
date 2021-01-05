using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// simple loot table that returns a random item based on probability weights
public class LootTable<T>
{
    // possible items to be returned
    private List<T> items;
    // cumulative probabilities for each item
    // probabilities are relative i.e. they can be any positive float
    private List<float> probs;

    // stored random object
    private System.Random random;

    public LootTable()
    {
        items = new List<T>();
        probs = new List<float>();
        random = new System.Random(System.DateTime.Now.Second * 1000 + System.DateTime.Now.Millisecond);
    }

    public void Add(T item, float prob)
    {
        if (prob <= 0.0f)
        {
            Debug.LogError("LootTable: non-positive prob");
            return;
        }
        items.Add(item);

        float maxProb;
        if (probs.Count == 0)
            maxProb = 0.0f;
        else
            maxProb = probs[probs.Count - 1];
        probs.Add(maxProb + prob);
    }

    public T Get()
    {
        if (items.Count == 0) throw new System.Exception("LootTable: empty table");
        float value = (float)random.NextDouble() * probs[probs.Count - 1];
        int choice = 0;
        while (value > probs[choice])
        {
            ++choice;
            if (choice == probs.Count - 1) break;
        }
        return items[choice];
    }

    public int Count()
    {
        return items.Count;
    }
}
