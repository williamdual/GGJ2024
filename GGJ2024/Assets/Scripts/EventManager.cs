using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum GameState {
        Round,
        CardAdd,
        startingDeckSelection
    }
    public GameState gameState;
    public Canvas cardCanvas;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Round;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void highlightCrowdMembers(CardTypeEnum cardType)
    /*Highlights crowd members using a color that coresponds with their cardtype preference */
    {
        CrowdMember[] allObjs = UnityEngine.Object.FindObjectsOfType<CrowdMember>();
        for (int i = 0; i < allObjs.Length; i++)
        {
            CrowdMember cm =  allObjs[i];
            if (cm.type == cardType)
            {
                SpriteRenderer sprite = cm.GetComponent<SpriteRenderer>();
                sprite.color = Globals.CardTypeToColor[cardType];
            }
        }

    }

    public void OfferCards(Card[] CardPool){
        int numOfCardsToOffer = 3;
        int numOfCardsToAccept = 2;
        List<int> selectedCardNums = new List<int>();
        for(int i = 0; i < numOfCardsToOffer; i++)
        {
            int selectedIndex = UnityEngine.Random.Range(0, CardPool.Length - 1);
            while (selectedCardNums.Contains(selectedIndex))
            {
                selectedIndex = UnityEngine.Random.Range(0, CardPool.Length - 1);
            }
            selectedCardNums.Add(selectedIndex);
        }
        //Now you have all of the indexes of the cards to offer (with no repatitions)
        //TODO Put card GUI and responce
        
    }

}
