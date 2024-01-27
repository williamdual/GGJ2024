using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float CARD_DRAW_SPEED           = 12.0f;

    private float SLOW_DOWN_DIST            = 5.0f;
    private float SLOW_CARD_DRAW_SPEED      = 8.0f;

    private float SLOW_DOWN_DIST_TWO        = 2.5f;
    private float SLOW_CARD_DRAW_SPEED_TWO  = 5.0f;

    //IMPORTANT: the top of the 'deck' will be the last element in the lists, since popping from back
    //of a list is way more efficient

    private Card[] cardLibrary;
    public Canvas cardCanvas;
    public GameObject cardPrefab;
    public int maxHandSize   = 8;
    public int startHandSize = 5;
    public Transform deckPosition;

    //should be size of maxHandSize
    public Transform[] cardPositions;

    private List<Card> deck;

    //parallel, moveCardObjects[i] represents if handCardObjects[i] should be moving towards its hand place
    private List<GameObject> handCardObjects;
    private bool[]           moveCardObjects;
    private List<Card> discard;
    private List<Card> hand;

    private TextMeshProUGUI discardText;
    private TextMeshProUGUI deckText;
    private bool canPlay;

    public static void Shuffle(List<Card> cardList) {
        int count   = cardList.Count;
        int last    = count - 1;
        
        for (int i = 0; i < last; ++i) {
            int r = UnityEngine.Random.Range(i, count);
            Card tmp = cardList[i];
            cardList[i] = cardList[r];
            cardList[r] = tmp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cardLibrary = Resources.LoadAll<Card>("Cards");

        Debug.Log(cardLibrary[0]);

        deck            = new List<Card>();
        discard         = new List<Card>();
        hand            = new List<Card>();
        handCardObjects = new List<GameObject>();
        deckText        = cardCanvas.gameObject.transform.Find("Deck/DeckText").GetComponent<TextMeshProUGUI>();
        discardText     = cardCanvas.gameObject.transform.Find("Discard/DiscardText").GetComponent<TextMeshProUGUI>();

        //init bool array
        moveCardObjects = new bool[maxHandSize];
        for(int i = 0; i < maxHandSize; i++){
            moveCardObjects[i] = false;
        }

        InitDeck();
        StartTurn();
    }

    public void AddCardToBottomDeck(Card newCard){
        Debug.Log("Adding card to bottom of deck: " + newCard.cardName);
        deck.Insert(0, newCard); 
    }

    public void AddCardToTopDeck(Card newCard){
        Debug.Log("Adding card to top of deck: " + newCard.cardName);
        deck.Insert(deck.Count, newCard); 
    }

    public void AddCardToDiscard(Card newCard){
        Debug.Log("Adding card to discard: " + newCard.cardName);
        discard.Insert(discard.Count, newCard); 
    }

    public void UpdateTexts(){
        discardText.text = "Discard: " + discard.Count.ToString();
        deckText.text    = "Deck: " + deck.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        float step          = CARD_DRAW_SPEED * Time.deltaTime;
        float slow_step     = SLOW_CARD_DRAW_SPEED * Time.deltaTime;
        float slow_step_two = SLOW_CARD_DRAW_SPEED_TWO * Time.deltaTime;

        for(int i = 0; i < handCardObjects.Count; i++){
            //move cards to where they're supposed to be
            if(true == moveCardObjects[i]){
                Vector2 target  = cardPositions[i].position;
                float dist      = Vector2.Distance(target,handCardObjects[i].transform.position);
                
                if(dist < SLOW_DOWN_DIST_TWO){
                    handCardObjects[i].transform.position = Vector2.MoveTowards(handCardObjects[i].transform.position, target, slow_step_two);
                }
                else if(dist < SLOW_DOWN_DIST){
                    handCardObjects[i].transform.position = Vector2.MoveTowards(handCardObjects[i].transform.position, target, slow_step);
                }
                else{
                    handCardObjects[i].transform.position = Vector2.MoveTowards(handCardObjects[i].transform.position, target, step);
                }
            }
        }
    }

    private void InitDeck(){
        for(int i = 0; i < 3; i++){
            AddCardToTopDeck(cardLibrary[i]);
            AddCardToTopDeck(cardLibrary[i]);
        }
    }

    public void StartTurn(){
        canPlay = false;
        int startSize = hand.Count;
        Debug.Log("Starting new turn.");
        int toDraw = startHandSize - hand.Count;
        if(toDraw < 0){toDraw=0;}
        StartCoroutine(DrawCards(toDraw, startSize));
    }

    public IEnumerator DrawCards(int numToDraw, int startSize){
        for(int i = 0; i < numToDraw; i++){
            _DrawCard();
            UpdateTexts();
        }

        Debug.Log("Hand count: " + hand.Count.ToString());

        for(int i = startSize; i < hand.Count; i++){
            //spawn in new card, add it to list so it moves to where it's supposed to go
            Debug.Log("Instantiating: " + hand[i].name);
            GameObject newCard = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, cardCanvas.transform);
            newCard.gameObject.GetComponent<CardDisplay>().card = hand[i];
            newCard.gameObject.GetComponent<CardDisplay>().SetupFromCard(this, i);
            handCardObjects.Add(newCard);
            yield return new WaitForSeconds(0.5f);
        }
        canPlay = true;
    }

    //should ONLY be called in DrawCards()
    private void _DrawCard(){
        //only draw if it won't exceed max
        if(hand.Count < maxHandSize){
            if(deck.Count == 0){
                if(discard.Count == 0){return;}
                for(int i = 0; i < discard.Count; i++){
                    deck.Add(discard[i]);
                }
                discard.Clear();
                GameManager.Shuffle(hand);
            }
        
            //draw
            int toDrawIndex = deck.Count-1;
            hand.Add(deck[toDrawIndex]);
            moveCardObjects[hand.Count-1] = true;
            deck.RemoveAt(toDrawIndex);
        }
    }

    public void SetCardCanMove(bool canMove, int listIndex){
        moveCardObjects[listIndex] = canMove;
    }

    public void PlayCard(int listIndex){
        //TODO: do effects, see if u have enough resources
        if(canPlay){
            GameObject cardObj      = handCardObjects[listIndex];
            CardDisplay cardScript  = cardObj.GetComponent<CardDisplay>();
            
            AddCardToDiscard(hand[listIndex]);
            hand.RemoveAt(listIndex);
            handCardObjects.RemoveAt(listIndex);


            AudioSource.PlayClipAtPoint(cardScript.ReturnAudioClip(), Vector3.zero);
            
            for(int i = listIndex; i < maxHandSize-1; i++){
                moveCardObjects[i] = moveCardObjects[i+1];
                if(i < handCardObjects.Count){
                    handCardObjects[i].GetComponent<CardDisplay>().SetListPos(i);
                }
            }

            UpdateTexts();
            Destroy(cardObj.gameObject);
        }
    }

    public void EndTurn(){
        //refresh energy
        StartTurn();
    }
}
