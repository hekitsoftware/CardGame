using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class HandScript : MonoBehaviour
{
    [Header("Instance References")]
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public GameManager gameManager;

    [Header("Hand")]
    [SerializeField] public List<GameObject> handSlots;

    //Card Pools
    List<CardID> commonCards = new List<CardID>();
    List<CardID> uncommonCards = new List<CardID>();
    List<CardID> rareCards = new List<CardID>();
    List<CardID> ultraCards = new List<CardID>();

    private void Start()
    {
        PopulatePools();
        RollHand();
    }

    public void PopulatePools()
    {
        foreach (CardID card in gameManager.Deck)
        {
            switch (card.Rarity)
            {
                case Rarity.Common:
                    commonCards.Add(card);
                    break;
                case Rarity.Uncommon:
                    uncommonCards.Add(card);
                    break;
                case Rarity.Rare:
                    rareCards.Add(card);
                    break;
                case Rarity.Ultra:
                    ultraCards.Add(card);
                    break;
            }
        }
    }

    public void RollHand()
    {
        for (int i = 0; i < handSlots.Count; i++)
        {
            if (handSlots[i].transform.childCount == 0)
            {
                List<CardID> cardPool = null;

                int randomNumber = UnityEngine.Random.Range(0, 100);
                if (randomNumber < 5)
                    cardPool = ultraCards;
                else if (randomNumber < 20)
                    cardPool = rareCards;
                else if (randomNumber < 50)
                    cardPool = uncommonCards;
                else
                    cardPool = commonCards;

                if (cardPool == null || cardPool.Count == 0)
                    continue; // safety check if pool is empty

                int cardSelection = UnityEngine.Random.Range(0, cardPool.Count);
                CardID cardID = cardPool[cardSelection];

                GameObject manifestCard = Instantiate(cardPrefab, handSlots[i].transform);
                manifestCard.transform.localPosition = Vector3.zero;

                Card card = manifestCard.GetComponent<Card>();
                card.SetupCard(cardID, CardFinish.Matte);
                card.dragTargetPos = handSlots[i].transform.localPosition;
            }
        }
    }

}
