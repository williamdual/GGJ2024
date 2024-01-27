using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {
    public string cardName;
    public string description;

    public CardTypeEnum cardType;
    public Sprite artwork;

    public int energyCost;
    public int healthCost;
    public int funnyValue;

}
