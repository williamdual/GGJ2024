using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CardTypeEnum {Family, Dark, Animals, Romance, Deprecating, Prop, Corny, None};

public class Globals{
public static Dictionary<CardTypeEnum, Color> CardTypeToColor = new Dictionary<CardTypeEnum, Color>(){
        {CardTypeEnum.Family, new Color(230, 126, 34,1)},
        {CardTypeEnum.Dark, new Color(108, 122, 137,1)},
        {CardTypeEnum.Animals, Color.green},
        {CardTypeEnum.Romance, new Color(239, 207, 227, 1)},
        {CardTypeEnum.Deprecating, new Color(191, 85, 236, 1)},
        {CardTypeEnum.Prop, new Color(3, 138, 255,1)},
        {CardTypeEnum.Corny, new Color(189, 195, 199, 1)}
};

public static Dictionary<CardTypeEnum, List<String>> CardTypeToNonStartingCardNames = new Dictionary<CardTypeEnum, List<String>>(){
        {CardTypeEnum.Family, new List<String>() {"Run", "LoveWife", "Listen", "Gang"}},
        {CardTypeEnum.Dark, new List<String>() {"Beep", "Calendar", "KillYou", "Graveyard"}},
        {CardTypeEnum.Animals, new List<String>() {"Squirrel", "DogFart", "DogPhoto", "Termite"}},
        {CardTypeEnum.Romance, new List<String>() {"LoveSong", "LovePotato", "LoveFart", "ThirdWife"}},
        {CardTypeEnum.Deprecating, new List<String>() {"Unemployed", "GaryAlcoholic", "Alcohol", "Circumsision"}}
};

}

