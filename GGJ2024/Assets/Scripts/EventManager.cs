using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public GameObject mainOptionCanvas;
    public GameObject deckSelectObj;
    private GameManager gameManager;

    public CardTypeEnum startingDeck;
    public int numOfCardsToOffer = 3; //Never go above 5
    List<Card> cardsToOffer = new List<Card>();
    List<int> selectedCardIndexes = new List<int>();
    public int numOfCardsToAccept = 2;
    int numOfCardsLeft = 0;
    public List<Transform> shopPositions;

    public enum GameState {
        Round,
        CardAdd,
        startingDeckSelection
    }
    public GameState gameState;
    public Canvas cardCanvas;

    Dictionary<CardTypeEnum, GameObject> preFabMap;

    // Start is called before the first frame update
    void Start()
    {

        gameManager = gameObject.GetComponent<GameManager>();
        gameState   = GameState.startingDeckSelection;
        mainOptionCanvas.SetActive(true);
        deckSelectObj.SetActive(true);

        preFabMap = gameManager.getPreFabMap();

        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void cycleState()
    {
        if(gameState ==  GameState.startingDeckSelection)
        {
            gameManager.InitDeck(startingDeck);
            gameState = GameState.Round;   
            gameManager.StartRound();
        }
        else if (gameState ==  GameState.CardAdd)
        {
            mainOptionCanvas.SetActive(false);
            gameState = GameState.Round;   
            gameManager.StartRound();
        }
        else if (gameState == GameState.Round)
        {
            mainOptionCanvas.SetActive(true);
            gameState = GameState.CardAdd;
            OfferCards(Resources.LoadAll<Card>("Cards"));
        }
        
    }

    public void SetStartingDeck(CardTypeEnum cardType){
        Time.timeScale = 1;
        deckSelectObj.SetActive(false);
        mainOptionCanvas.SetActive(false);
        startingDeck = cardType;
        cycleState();
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
                cm.Highlight(Globals.CardTypeToColor[cardType]);
            }
        }

    }

    public void OfferCards(Card[] CardPool){
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
        for(int i = 0; i < selectedCardNums.Count; i++)
        {
            cardsToOffer.Add(CardPool[selectedCardNums[i]]);
        }
        numOfCardsLeft = numOfCardsToAccept;
        //Now you have all of the indexes of the cards to offer (with no repatitions)
        //TODO Put card GUI and responce
        for(int i = 0; i < cardsToOffer.Count; i++){
            GameObject cardPrefab = gameManager.getPreFabMap()[cardsToOffer[i].cardType];
            Debug.Log("CARD Select PREFAB: " + cardPrefab.name);
            //spawn in new card, add it to list so it moves to where it's supposed to go
            GameObject newCard = Instantiate(cardPrefab, shopPositions[i].position, Quaternion.identity, mainOptionCanvas.transform);
            newCard.gameObject.GetComponent<CardDisplay>().card = cardsToOffer[i];
            newCard.gameObject.GetComponent<CardDisplay>().setUpCardShop(this, i);
        }
    }
    public void selectCard(int listPos)
    {
        if(!selectedCardIndexes.Contains(listPos))
        {
            Card cardToAdd = cardsToOffer[listPos];     
            selectedCardIndexes.Add(listPos);
            numOfCardsLeft--;
            gameManager.deck.Add(cardToAdd);
        }
        if(numOfCardsLeft <= 0)
        {
            cardsToOffer.Clear();
            selectedCardIndexes.Clear();
            cycleState();
        }
    }
}
