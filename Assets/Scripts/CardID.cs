using UnityEngine;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Ultra
}

[CreateAssetMenu(fileName = "CardID", menuName = "Scriptable Objects/CardID")]
public class CardID : ScriptableObject
{
    [Header("Identity")]
    public Rarity Rarity;
    public string Name;
    [TextArea] public string Description;
    public Sprite artwork;

    [Header("Game Stats")]
    public int value;
}
