using UnityEngine;
using System.Collections.Generic;

public class OpponentHandScript : MonoBehaviour
{
    [Header("Instance References")]
    [SerializeField] private GameObject cardBackPrefab; // prefab with just card back
    [SerializeField] private GameManager gameManager;

    [Header("Hand")]
    [SerializeField] public List<GameObject> handSlots; // slots across the top
    [SerializeField] private List<Card> cardsInHand = new List<Card>();
}
