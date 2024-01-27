using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        Debug.Log("Starting new turn.");
        StartCoroutine(DrawCards(startHandSize));
    }

    public IEnumerator DrawCards(int numToDraw){
        for(int i = 0; i < numToDraw; i++){
            _DrawCard();
        }

        Debug.Log("Hand count: " + hand.Count.ToString());

        for(int i = 0; i < hand.Count; i++){
            //spawn in new card, add it to list so it moves to where it's supposed to go
            Debug.Log("Instantiating: " + hand[i].name);
            GameObject newCard = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, cardCanvas.transform);
            newCard.gameObject.GetComponent<CardDisplay>().card = hand[i];
            newCard.gameObject.GetComponent<CardDisplay>().SetupFromCard();
            handCardObjects.Add(newCard);
            yield return new WaitForSeconds(0.5f);
        }
    }

    //should ONLY be called in DrawCards()
    private void _DrawCard(){
        //only draw if it won't exceed max
        if(hand.Count < maxHandSize){
            if(deck.Count == 0){
                if(discard.Count == 0){return;}
                for(int i = 0; i < discard.Count; i++){
                    deck[i] = discard[i];
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
}
