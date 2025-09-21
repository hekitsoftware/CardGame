using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardInstance
{
    public CardID cardID;
    public CardFinish finish;

    public CardInstance(CardID id, CardFinish f)
    {
        cardID = id;
        finish = f;
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerID;
    public List<CardInstance> hand = new List<CardInstance>();
    // ^ Now uses CardInstance so when you draw a card, you save both the rarity-based card + its finish info
    public int score;
    public bool isLocalPlayer;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] public float gameSpeed;
    [SerializeField] public float cardSpeed;

    public List<CardID> Deck;

    public PlayerData player1;
    public PlayerData player2;

    public HandScript localHand;
    public OpponentHandScript opponentHand;

    private void Start()
    {
        // Example: assume the local player ID is "P1"
        SetupPlayers("P1");

        // Deal starting hands
        DealStartingHands();
    }

    public void SetupPlayers(string localPlayerID)
    {
        // Example: assume IDs are "P1" and "P2" from server
        player1.isLocalPlayer = player1.playerID == localPlayerID;
        player2.isLocalPlayer = player2.playerID == localPlayerID;
    }

    public PlayerData GetLocalPlayer()
    {
        return player1.isLocalPlayer ? player1 : player2;
    }

    public PlayerData GetOpponent()
    {
        return player1.isLocalPlayer ? player2 : player1;
    }
    public void DealStartingHands(int handSize = 7)
    {
        PlayerData local = GetLocalPlayer();
        PlayerData opponent = GetOpponent();

        for (int i = 0; i < handSize; i++)
        {
            local.hand.Add(DrawRandomCard());
            opponent.hand.Add(DrawRandomCard());
        }

        RefreshHands();
    }

    public void RefreshHands()
    {
        localHand.RenderHand(GetLocalPlayer());
        opponentHand.RenderHand(GetOpponent());
    }

    public CardInstance DrawRandomCard()
    {
        // --- Pick rarity ---
        int rarityRoll = UnityEngine.Random.Range(0, 100);
        List<CardID> pool = null;

        if (rarityRoll < 5)
            pool = Deck.FindAll(c => c.Rarity == Rarity.Ultra);   // 5%
        else if (rarityRoll < 20)
            pool = Deck.FindAll(c => c.Rarity == Rarity.Rare);    // 15%
        else if (rarityRoll < 50)
            pool = Deck.FindAll(c => c.Rarity == Rarity.Uncommon);// 30%
        else
            pool = Deck.FindAll(c => c.Rarity == Rarity.Common);  // 50%

        // fallback if pool empty
        if (pool.Count == 0) pool = Deck;

        CardID chosenID = pool[UnityEngine.Random.Range(0, pool.Count)];

        // --- Pick finish ---
        int finishRoll = UnityEngine.Random.Range(0, 100);
        CardFinish finish;

        if (finishRoll < 3)         // 3%
            finish = CardFinish.Chroma;
        else if (finishRoll < 8)    // 5%
            finish = CardFinish.Void;
        else if (finishRoll < 20)   // 12%
            finish = CardFinish.Inverse;
        else if (finishRoll < 50)   // 30%
            finish = CardFinish.Foil;
        else                        // 50%
            finish = CardFinish.Matte;

        // Special rule: REDD can't be Chroma
        if (chosenID.Name == "REDD" && finish == CardFinish.Chroma)
            finish = CardFinish.Void;

        return new CardInstance(chosenID, finish);
    }
}
