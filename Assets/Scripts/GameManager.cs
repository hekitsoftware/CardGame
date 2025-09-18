using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float gameSpeed;
    [SerializeField] public float cardSpeed;
    [SerializeField] public HandScript handScript;

    [SerializeField] public List<CardID> Deck;
    [SerializeField] public List<Material> finishPool;
}
