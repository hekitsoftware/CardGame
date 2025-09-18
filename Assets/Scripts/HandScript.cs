using UnityEngine;
using System.Collections.Generic;

public class HandScript : MonoBehaviour
{
    [Header("Instance References")]
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public GameManager gameManager;

    [Header("Hand")]
    [SerializeField] public List<GameObject> handSlots;
    [SerializeField] public List<Card> cardsInHand = new List<Card>();

    // Card Pools
    List<CardID> commonCards = new List<CardID>();
    List<CardID> uncommonCards = new List<CardID>();
    List<CardID> rareCards = new List<CardID>();
    List<CardID> ultraCards = new List<CardID>();

    private void Start()
    {
        PopulatePools();
        RollHand();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReRoll();
        }
    }

    public void PopulatePools()
    {
        foreach (CardID card in gameManager.Deck)
        {
            switch (card.Rarity)
            {
                case Rarity.Common: commonCards.Add(card); break;
                case Rarity.Uncommon: uncommonCards.Add(card); break;
                case Rarity.Rare: rareCards.Add(card); break;
                case Rarity.Ultra: ultraCards.Add(card); break;
            }
        }
    }

    public void RollHand()
    {
        for (int i = 0; i < handSlots.Count; i++)
        {
            if (handSlots[i].transform.childCount > 0) continue;

            //CARD BODY >>>>>>>>>>>>
            List<CardID> cardPool = null;
            int randomNumber = UnityEngine.Random.Range(0, 100);

            if (randomNumber < 5)
                cardPool = ultraCards; // 5% chance
            else if (randomNumber < 20)
                cardPool = rareCards; // 15% chance
            else if (randomNumber < 50)
                cardPool = uncommonCards; // 30% chance
            else
                cardPool = commonCards; // 50% chance

            // fallback if selected pool is empty
            if (cardPool == null || cardPool.Count == 0)
            {
                if (commonCards.Count > 0) cardPool = commonCards;
                else if (uncommonCards.Count > 0) cardPool = uncommonCards;
                else if (rareCards.Count > 0) cardPool = rareCards;
                else if (ultraCards.Count > 0) cardPool = ultraCards;
                else continue; // no cards left
            }

            //CARD FINISH >>>>>>>>>>>>
            CardFinish cardFinish = new CardFinish();

            int newRandomNumber = UnityEngine.Random.Range(0, 100);

            if (newRandomNumber < 3)               // 3% chance
                cardFinish = CardFinish.Chroma;
            else if (newRandomNumber < 8)          // 5% chance
                cardFinish = CardFinish.Void;
            else if (newRandomNumber < 20)         // 12% chance
                cardFinish = CardFinish.Inverse;
            else if (newRandomNumber < 50)         // 30% chance
                cardFinish = CardFinish.Foil;
            else                                   // 50% chance
                cardFinish = CardFinish.Matte;

            int cardSelection = UnityEngine.Random.Range(0, cardPool.Count);
            CardID cardID = cardPool[cardSelection];

            GameObject manifestCard = Instantiate(cardPrefab, handSlots[i].transform);
            manifestCard.transform.localPosition = Vector3.zero;

            Card card = manifestCard.GetComponent<Card>();
            card.parentSlot = handSlots[i].transform;
            card.dragTargetPos = handSlots[i].transform.position;

            //REDD can't be a Chroma
            if(cardID.Name == "REDD" && cardFinish == CardFinish.Chroma)
            {
                cardFinish = CardFinish.Void;
            }
            card.SetupCard(cardID, cardFinish);

            cardsInHand.Add(card);
        }
    }

    public void ReRoll()
    {
        // Destroy all existing cards
        foreach (Card card in cardsInHand)
        {
            if (card != null)
            {
                // Stop DOTween tweens before destroying
                card.StopIdleRotation();
                Destroy(card.gameObject);
            }
        }

        // Clear the hand list
        cardsInHand.Clear();

        // Optionally, clear children from slots (just to be safe)
        foreach (GameObject slot in handSlots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Populate the hand again
        RollHand();
    }
}
