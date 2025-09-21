using UnityEngine;
using System.Collections.Generic;

public class HandScript : MonoBehaviour
{
    [Header("Instance References")]
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public GameManager gameManager;

    [Header("Hand Slots")]
    [SerializeField] public List<GameObject> handSlots;
    [SerializeField] public List<Card> cardsInHand = new List<Card>();

    // Render a player's hand based on their PlayerData.
    // Local player sees cards face-up; opponent sees backs.
    public virtual void RenderHand(PlayerData playerData)
    {
        ClearHand();

        for (int i = 0; i < playerData.hand.Count && i < handSlots.Count; i++)
        {
            GameObject manifestCard = Instantiate(cardPrefab, handSlots[i].transform);
            manifestCard.transform.localPosition = Vector3.zero;

            Card card = manifestCard.GetComponent<Card>();
            card.parentSlot = handSlots[i].transform;
            card.dragTargetPos = handSlots[i].transform.position;

            // Local player sees cards face-up; opponent sees back
            card.hiddenFromPlayer = !playerData.isLocalPlayer;

            CardInstance instance = playerData.hand[i];
            card.SetupCard(instance.cardID, instance.finish);
            card.Render();

            cardsInHand.Add(card);
        }
    }

    // Clears the current cards in the hand, destroying the GameObjects.
    public void ClearHand()
    {
        // Destroy spawned card objects
        foreach (Card c in cardsInHand)
        {
            if (c != null)
                Destroy(c.gameObject);
        }
        cardsInHand.Clear();

        // Destroy any leftover children in slots (safety)
        foreach (GameObject slot in handSlots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
