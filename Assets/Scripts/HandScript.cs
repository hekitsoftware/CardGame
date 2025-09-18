using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;

// ------------------------------
// Helper Classes for Weighted Random
// ------------------------------
[System.Serializable]
public class RarityChance
{
    public Rarity rarity;       // The rarity of a card (Common, Rare, etc.)
    [Range(0f, 1f)] public float chance; // Weight/probability for spawning
}

[System.Serializable]
public class FinishChance
{
    public CardFinish finish;   // The finish type of the card (Matte, Holo, etc.)
    [Range(0f, 1f)] public float chance; // Weight/probability for spawning
}

// ------------------------------
// HandScript: Spawns cards into hand
// ------------------------------
public class HandScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rect; // RectTransform of the hand container

    [Header("Prefabs")]
    [SerializeField] private GameObject handSlotPrefab; // Empty slot prefab (for layout)
    [SerializeField] private Card cardPrefab; // Generic card prefab to spawn

    [Header("Card Pool")]
    [SerializeField] private List<CardID> allCardIDs; // All cards in the game

    [Header("Rarity Chances")]
    [SerializeField] private List<RarityChance> cardRarityChances; // Chance per card rarity
    [SerializeField] private List<FinishChance> finishRarityChances; // Chance per finish

    [Header("Spawn Settings")]
    [SerializeField][Range(1, 10)] private int cardsToSpawn = 5; // Number of cards in hand

    private void Start()
    {
        rect ??= GetComponent<RectTransform>();
        SpawnHand();
    }

    private void SpawnHand()
    {
        // Loop for each card to spawn
        for (int i = 0; i < cardsToSpawn; i++)
        {
            GameObject slot = Instantiate(handSlotPrefab, transform);
            Rarity selectedRarity = GetRandomWeighted(cardRarityChances, rc => rc.chance).rarity;

            List<CardID> possibleCards = allCardIDs.Where(c => c.Rarity == selectedRarity).ToList();

            if (possibleCards.Count == 0)
            {
                // Safety check: if no card exists for this rarity
                Debug.LogWarning("No cards available for rarity " + selectedRarity);
                continue; // Skip this iteration
            }

            // Randomly pick one card from filtered list
            CardID selectedCardID = possibleCards[UnityEngine.Random.Range(0, possibleCards.Count)];

            //Pick finish
            CardFinish selectedFinish = GetRandomWeighted(finishRarityChances, fc => fc.chance).finish;

            //Instantiate chosen card
            Card newCard = Instantiate(cardPrefab, slot.transform);
            newCard.ID = selectedCardID;       // Assign the chosen CardID
            newCard.finish = selectedFinish;   // Assign the chosen Finish
            newCard.ApplyFinish(selectedCardID); // Apply artwork and material
        }
    }

    // Select one item from a list based on weighted probabilities
    private T GetRandomWeighted<T>(List<T> items, Func<T, float> weightSelector)
    {
        // 1. Sum all weights
        float totalWeight = items.Sum(weightSelector);

        // 2. Pick a random value between 0 and total weight
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        // 3. Loop through the items and accumulate weight
        float sum = 0f;
        foreach (T item in items)
        {
            sum += weightSelector(item);
            if (randomValue <= sum)
                return item; // Return the first item where accumulated weight exceeds randomValue
        }

        // Fallback (should not happen, but safe)
        return items[items.Count - 1];
    }
}
