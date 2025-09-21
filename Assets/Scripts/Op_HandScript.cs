using UnityEngine;

public class OpponentHandScript : HandScript
{
    public override void RenderHand(PlayerData playerData)
    {
        ClearHand();

        for (int i = 0; i < playerData.hand.Count && i < handSlots.Count; i++)
        {
            GameObject manifestCard = Instantiate(cardPrefab, handSlots[i].transform);
            manifestCard.transform.localPosition = Vector3.zero;

            Card card = manifestCard.GetComponent<Card>();
            card.parentSlot = handSlots[i].transform;
            card.dragTargetPos = handSlots[i].transform.position;

            // Make sure it's always hidden to the local player
            card.hiddenFromPlayer = true;

            // Setup and render the card after hiding it
            CardInstance instance = playerData.hand[i];
            card.SetupCard(instance.cardID, instance.finish);

            // Force render after setting hidden state
            card.Render();

            cardsInHand.Add(card);
        }
    }
}