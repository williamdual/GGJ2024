using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public GameObject mainOptionCanvas;
    public GameObject deckSelectObj;
    private GameManager gameManager;

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
        gameManager = gameObject.GetComponent<GameManager>();
        gameState   = GameState.startingDeckSelection;
        mainOptionCanvas.SetActive(true);
        deckSelectObj.SetActive(true);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartingDeck(CardTypeEnum cardType){
        Time.timeScale = 1;
        deckSelectObj.SetActive(false);
        mainOptionCanvas.SetActive(false);
        gameManager.StartRound(cardType);
        gameState = GameState.Round;
    }

    public void SetStartingDeckString(String strCardType){
        //unity is dumb need to get around can't call above function from btn press
        //because it doesn't see CardTypeEnum as a thing that can be in a function
        //or whatever
        if(strCardType == CardTypeEnum.Dark.ToString()){
            SetStartingDeck(CardTypeEnum.Dark);
        }
        else if(strCardType == CardTypeEnum.Romance.ToString()){
            SetStartingDeck(CardTypeEnum.Romance);
        }
        else if(strCardType == CardTypeEnum.Animals.ToString()){
            SetStartingDeck(CardTypeEnum.Animals);
        }
        else if(strCardType == CardTypeEnum.Family.ToString()){
            SetStartingDeck(CardTypeEnum.Family);
        }
        else if(strCardType == CardTypeEnum.Deprecating.ToString()){
            SetStartingDeck(CardTypeEnum.Deprecating);
        }
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
