using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

    [Header("Normal Attributes")]
    public string cardName;
    public string description;
    public bool isCrowdwork;

    public CardTypeEnum cardType;
    public Sprite artwork;

    public int energyCost = 0;
    public int funnyValue = 0;
    public float minFunnyValuePercentRange = 0;
    public float maxFunnyValuePercentRange = 0;

    public int healthCost = 0;
    public float percentChanceToTakeLessHP = 0;
    public int luckyHealthCost = 0;

    public AudioClip audioClip;


    [Header("Special Attributes")]
    public int costEmotion = 0;

    public int addOvercharge = 0;
    public int addEnergy = 0;
    public int addEmotion = 0;
    public int addHealth = 0;
    public bool stale = false;

    //these are linked
    public int drawFromDiscard = 0;
    public int drawRandomFromDiscard = 0;

    public int discardRandomCards = 0;
    public bool endTurnOnDiscardFailure = false;

    public bool entersSuspense = false;
    public bool endTurnIfNotSuspense = false;

}
