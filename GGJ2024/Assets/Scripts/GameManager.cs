using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private GameObject tutPopup;
    private int numHappyThreshold = 1;
    private List<Card> stalePile;
    private EventManager eventManager;

    private GameObject gameOverObj;
    private TextMeshProUGUI gameOverText;
    private int peopleHappy = 0;
    private float CARD_DRAW_SPEED           = 12.0f;

    private float SLOW_DOWN_DIST            = 5.0f;
    private float SLOW_CARD_DRAW_SPEED      = 8.0f;

    private float SLOW_DOWN_DIST_TWO        = 2.5f;
    private float SLOW_CARD_DRAW_SPEED_TWO  = 5.0f;

    //IMPORTANT: the top of the 'deck' will be the last element in the lists, since popping from back
    //of a list is way more efficient

    public Dictionary<String, Card> cardLibrary;
    public Canvas cardCanvas;
    public GameObject cornyPrefab;
    public GameObject familyPrefab;
    public GameObject darkPrefab;
    public GameObject animalsPrefab;
    public GameObject romancePrefab;
    public GameObject deprecatingPrefab;
    public GameObject propPrefab;
    public int maxHandSize   = 8;
    public int startHandSize = 5;
    public Transform deckPosition;

    //should be size of maxHandSize
    public Transform[] cardPositions;

    private TextMeshProUGUI energyText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI emotionText;
    private TextMeshProUGUI overchargeText;
    private GameObject      suspenseObj;

    public List<Card> deck;

    //parallel, moveCardObjects[i] represents if handCardObjects[i] should be moving towards its hand place
    private List<GameObject> handCardObjects;
    private bool[]           moveCardObjects;
    private List<Card> discard;
    private List<Card> hand;

    private TextMeshProUGUI discardText;
    private TextMeshProUGUI deckText;
    private bool canPlay;

    private int curEnergy = 3;
    private int maxHealth = 6;
    private int curHealth;
    private int curEmotion = 0;
    private int curOvercharge = 0;
    private bool curSuspense = false;

    [SerializeField] private Spawner spawnCrowdMember;
    private Dictionary<CardTypeEnum, GameObject> prefabMap;

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
        stalePile = new List<Card>();
        prefabMap = getPreFabMap();

        cardLibrary = new Dictionary<string, Card>();

        Card[] cardArr = Resources.LoadAll<Card>("Cards");
        for(int i = 0; i < cardArr.Length; i++){
            cardLibrary.Add(cardArr[i].name, cardArr[i]);
        }

        curHealth       = maxHealth;
        deck            = new List<Card>();
        discard         = new List<Card>();
        hand            = new List<Card>();
        handCardObjects = new List<GameObject>();
        deckText        = cardCanvas.gameObject.transform.Find("Deck/DeckText").GetComponent<TextMeshProUGUI>();
        discardText     = cardCanvas.gameObject.transform.Find("Discard/DiscardText").GetComponent<TextMeshProUGUI>();
        energyText      = cardCanvas.gameObject.transform.Find("Energy/EnergyText").GetComponent<TextMeshProUGUI>();
        healthText      = cardCanvas.gameObject.transform.Find("Health/HealthText").GetComponent<TextMeshProUGUI>();
        emotionText     = cardCanvas.gameObject.transform.Find("Emotion/EmotionText").GetComponent<TextMeshProUGUI>();
        overchargeText  = cardCanvas.gameObject.transform.Find("Overcharge/OverchargeText").GetComponent<TextMeshProUGUI>();
        suspenseObj     = cardCanvas.gameObject.transform.Find("Suspense").gameObject;
        gameOverObj     = cardCanvas.gameObject.transform.Find("GameOver").gameObject;
        gameOverText    = cardCanvas.gameObject.transform.Find("GameOver/GameOverText").gameObject.GetComponent<TextMeshProUGUI>();
        tutPopup        = cardCanvas.gameObject.transform.Find("Tutorial").gameObject;
        
        //init bool array
        moveCardObjects = new bool[maxHandSize];
        for(int i = 0; i < maxHandSize; i++){
            moveCardObjects[i] = false;
        }

        eventManager = gameObject.GetComponent<EventManager>(); 
    }

    public Dictionary<CardTypeEnum, GameObject> getPreFabMap()
    {
        return new Dictionary<CardTypeEnum, GameObject>(){
        {CardTypeEnum.Family, familyPrefab},
        {CardTypeEnum.Dark, darkPrefab},
        {CardTypeEnum.Animals, animalsPrefab},
        {CardTypeEnum.Romance, romancePrefab},
        {CardTypeEnum.Deprecating, deprecatingPrefab},
        {CardTypeEnum.Prop, propPrefab},
        {CardTypeEnum.Corny, cornyPrefab}};
    }

    public void RegenerateDeck(){
        for(int i = 0; i < stalePile.Count; i++){
            AddCardToTopDeck(stalePile[i]);
        }
        for(int i = 0; i < discard.Count; i++){
            AddCardToTopDeck(discard[i]);
        }
        for(int i = 0; i < hand.Count; i++){
            Destroy(handCardObjects[i]);
            AddCardToTopDeck(hand[i]);
        }

        handCardObjects.Clear();
        hand.Clear();
        discard.Clear();
        stalePile.Clear();

        Shuffle(deck);
    }

    public void AddPeopleHappy(int num){
        peopleHappy += num;
        if(peopleHappy >= numHappyThreshold){
            numHappyThreshold = numHappyThreshold + numHappyThreshold + 1;
            eventManager.cycleState();
        }
    }

    public void EndTut(){
        tutPopup.SetActive(false);
    }

    public void StartRound(){
        //Set up for new set/round
        Shuffle(deck);
        curEnergy     = 3;
        curOvercharge = 0;
        curEmotion    = 0;
        curHealth     += 1;
        if(curHealth > maxHealth){
            curHealth = maxHealth;
        }
        curSuspense   = false;
        UpdateTexts();
        StartTurn();
    }

    public bool GetCanPlay(){
        return canPlay;
    }

    public void AddCardToBottomDeck(Card newCard){
        deck.Insert(0, newCard); 
    }

    public void AddCardToTopDeck(Card newCard){
        deck.Insert(deck.Count, newCard); 
    }

    public void AddCardToDiscard(Card newCard){
        discard.Insert(discard.Count, newCard); 
    }

    public void AddToStalePile(Card newCard){
        stalePile.Add(newCard);
    }

    public void UpdateTexts(){
        discardText.text   = discard.Count.ToString();
        deckText.text      = deck.Count.ToString();

        energyText.text      = curEnergy.ToString();
        healthText.text      = curHealth.ToString() + "/" + maxHealth.ToString();
        emotionText.text     = curEmotion.ToString();
        overchargeText.text  = curOvercharge.ToString();

        suspenseObj.SetActive(false);
        if(curSuspense){
            suspenseObj.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(eventManager.gameState == EventManager.GameState.Round)
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
    }

    public void InitDeck(CardTypeEnum cardType){
        AddCardToTopDeck(cardLibrary["KnockKnock"]);
        AddCardToTopDeck(cardLibrary["HonkHonk"]);
        AddCardToTopDeck(cardLibrary["EvilStairs"]);
        AddCardToTopDeck(cardLibrary["StrikeOut"]);
        AddCardToTopDeck(cardLibrary["Noise"]);
        AddCardToTopDeck(cardLibrary["DogMath"]);

        List<String> cardsToAdd = Globals.CardTypeToNonStartingCardNames[cardType];
        for(int i = 0; i < cardsToAdd.Count; i++){
            AddCardToTopDeck(cardLibrary[cardsToAdd[i]]);
        }

        GameManager.Shuffle(deck);
    }

    public void StartTurn(){
        canPlay = false;
        int toDraw = startHandSize - hand.Count;
        if(toDraw < 0){toDraw=0;}
        StartCoroutine(DrawCards(toDraw));
        //20% 3, 50% 2, 30% 1
        List<int> numToSpawn = new List<int>(){1,1,1, 2,2,2,2,2, 3,3};
        int index = UnityEngine.Random.Range(0, numToSpawn.Count);
        for (int i = 0; i < numToSpawn[index]; i++)
        {
            spawnCrowdMember.Spawn();
        }

    }

    public void DiscardRandomCards(int numToDiscard){
        for(int i = 0; i < numToDiscard; i++){
            if(hand.Count > 0){
                int indexToRemove = UnityEngine.Random.Range(0, hand.Count);
                DiscardCardAtIndex(indexToRemove, true);
            }
        }
    }

    public void DiscardCardAtIndex(int listIndex, bool putInDiscardPile){
        Card cardToAdd = hand[listIndex];
        hand.RemoveAt(listIndex);
        handCardObjects.RemoveAt(listIndex);
        
        for(int i = listIndex; i < maxHandSize-1; i++){
            moveCardObjects[i] = moveCardObjects[i+1];
            if(i < handCardObjects.Count){
                handCardObjects[i].GetComponent<CardDisplay>().SetListPos(i);
            }
        }

        if(putInDiscardPile){
            AddCardToDiscard(cardToAdd);
        }
    }

    public IEnumerator DrawCards(int numToDraw, bool random=false, bool fromDiscard=false){
        int startSize = hand.Count;
        for(int i = 0; i < numToDraw; i++){
            if(fromDiscard){
                _DrawCardFromDiscard(random);
            }
            else{
                _DrawCardFromDeck(random);
            }
            UpdateTexts();
        }

        for(int i = startSize; i < hand.Count; i++){
            GameObject cardPrefab = prefabMap[hand[i].cardType];
            Debug.Log("CARD PREFAB: " + cardPrefab.name);
            //spawn in new card, add it to list so it moves to where it's supposed to go
            GameObject newCard = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, cardCanvas.transform);
            newCard.gameObject.GetComponent<CardDisplay>().card = hand[i];
            newCard.gameObject.GetComponent<CardDisplay>().SetupFromCard(this, i);
            handCardObjects.Add(newCard);
            yield return new WaitForSeconds(0.5f);
        }
        canPlay = true;
    }

    //should ONLY be called in DrawCards()
    private void _DrawCardFromDiscard(bool random=false){
        //only draw if it won't exceed max
        if(hand.Count < maxHandSize){
            if(discard.Count == 0){return;}
        
            //draw
            int toDrawIndex = discard.Count-1;
            if(random){
                toDrawIndex = UnityEngine.Random.Range(0, discard.Count);
            }
            hand.Add(discard[toDrawIndex]);
            moveCardObjects[hand.Count-1] = true;
            discard.RemoveAt(toDrawIndex);
        }
    }

    //should ONLY be called in DrawCards()
    private void _DrawCardFromDeck(bool random=false){
        //only draw if it won't exceed max
        if(hand.Count < maxHandSize){
            if(deck.Count == 0){
                if(discard.Count == 0){return;}
                for(int i = 0; i < discard.Count; i++){
                    deck.Add(discard[i]);
                }
                discard.Clear();
                GameManager.Shuffle(deck);
            }
        
            //draw
            int toDrawIndex = deck.Count-1;
            if(random){
                toDrawIndex = UnityEngine.Random.Range(0, deck.Count);
            }
            hand.Add(deck[toDrawIndex]);
            moveCardObjects[hand.Count-1] = true;
            deck.RemoveAt(toDrawIndex);
        }
    }

    public void SetCardCanMove(bool canMove, int listIndex){
        moveCardObjects[listIndex] = canMove;
    }

    public void PlayCard(int listIndex){
        //TODO: dont forget crowdwork effect

        if(canPlay){
            GameObject cardObj      = handCardObjects[listIndex];
            CardDisplay cardScript  = cardObj.GetComponent<CardDisplay>();
            Card cardToAdd          = hand[listIndex];

            if(cardScript.card.costEmotion > curEmotion || cardScript.card.energyCost > curEnergy + curOvercharge){
                //bad sound (didn't play the card due to not enough resources)
                return;
            }

            //means card is being played
            int totalDmg   = cardScript.card.funnyValue;
            float randNum  = UnityEngine.Random.Range(cardScript.card.minFunnyValuePercentRange, cardScript.card.maxFunnyValuePercentRange)/100;
            totalDmg = (int)(totalDmg * randNum);
            if(curSuspense){
                totalDmg = (int)(totalDmg * 1.5f);
            }
            bool disableSuspense = false;
            if(totalDmg != 0){
                disableSuspense = true;
                CrowdMember[] allObjs = UnityEngine.Object.FindObjectsOfType<CrowdMember>();
                for (int i = 0; i < allObjs.Length; i++)
                {
                    CrowdMember cm =  allObjs[i];
                    
                    cm.TakeDamage(totalDmg, cardScript.card.cardType);
                }
            }
            

            curEmotion      -= cardScript.card.costEmotion;
            curOvercharge   -= cardScript.card.energyCost;
            if(curOvercharge < 0){
                curEnergy += curOvercharge;
                curOvercharge = 0;
            }

            float rand  = UnityEngine.Random.Range(0.0f, 100.0f);
            int hpCost  = cardScript.card.healthCost;

            if(cardScript.card.percentChanceToTakeLessHP >= rand){
                hpCost = cardScript.card.luckyHealthCost;
            }

            TakeDamage(hpCost);
            if(cardScript.card.isCrowdwork){ eventManager.highlightCrowdMembers(cardScript.card.cardType);}
            //good sound (played the card)
            AudioSource.PlayClipAtPoint(cardScript.ReturnAudioClip(), Vector3.zero);

            
            DiscardCardAtIndex(listIndex, false);
            

            bool endTurn = DoCardEffects(cardScript);
            if(!cardToAdd.stale){
                AddCardToDiscard(cardToAdd);
            }
            else{
                AddToStalePile(cardToAdd);
            }

            if(disableSuspense){
                curSuspense = false;
            }
            UpdateTexts();
            Destroy(cardObj.gameObject);

            if(endTurn){
                EndTurn();
            }
        }
    }

    public void TakeDamage(int damage){
        //animations/fx?
        curHealth -= damage;
        if(curHealth<=0){
            GameOver();
        }
    }

    public void Restart(){
        String currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void GameOver(){
        Debug.Log("GAME OVER!");
        gameOverObj.SetActive(true);
        gameOverText.text = peopleHappy.ToString();
    }

    private bool DoCardEffects(CardDisplay card){
        bool autoEndTurn = false;

        curOvercharge += card.card.addOvercharge;
        curEnergy     += card.card.addEnergy;
        curEmotion    += card.card.addEmotion;
        curHealth     += card.card.addHealth;

        if(card.card.endTurnOnDiscardFailure && hand.Count < card.card.discardRandomCards){
            autoEndTurn = true;
        }
        if(!curSuspense && card.card.endTurnIfNotSuspense){
            autoEndTurn = true;
        }
        if(card.card.endsTurn){
            autoEndTurn = true;
        }
        if(card.card.entersSuspense){
            curSuspense = true;
        }
        
        DrawCards(card.card.drawFromDiscard, false, true);
        DrawCards(card.card.drawRandomFromDiscard, true, true);
        DiscardRandomCards(card.card.discardRandomCards);

        return autoEndTurn;
    }

    public void EndTurn(){

        if(eventManager.gameState == EventManager.GameState.Round && canPlay){
            //refresh energy
            List<CrowdMember> cms = FindObjectsOfType<CrowdMember>().ToList();
            foreach (CrowdMember cm in cms)
            {
                cm.TakeDamage(-2, CardTypeEnum.None);
            }
            curOvercharge = 0;
            curEnergy += 3;
            UpdateTexts();
            StartTurn();
        }
    }
}
