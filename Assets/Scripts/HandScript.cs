using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;

// Helper Classes for Weighted Random
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

//Spawns cards into hand
public class HandScript : MonoBehaviour
{
    //it's all nuked
}
