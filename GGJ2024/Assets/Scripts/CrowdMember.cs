using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[SerializeField] public enum Mood
{
    Ecstatic,
    Happy,
    Neutral,
    Sad,
    Angry
}
public class CrowdMember : MonoBehaviour 
{

    private List<String> happyQuotes;
    private List<String> sadQuotes;

    private float ecstaticThreshold = 0.80f;
    private float happyThreshold    = 0.60f;
    private float neutralThreshold  = 0.40f;
    private float sadThreshold      = 0.20f;
    private float angryThreshold    = 0.00f;
    [SerializeField] private Mood currMood = Mood.Neutral;

    public GameObject textPrefab;
    [SerializeField] private ParticleSystem confetti;

    [SerializeField] private List<Sprite> faces;
    private Dictionary<CardTypeEnum, CardTypeEnum[]> dislikes = new Dictionary<CardTypeEnum, CardTypeEnum[]>(){
    {CardTypeEnum.Family, new CardTypeEnum[] {CardTypeEnum.Dark}},
    {CardTypeEnum.Dark, new CardTypeEnum[] {CardTypeEnum.Family, CardTypeEnum.Animals, CardTypeEnum.Romance, CardTypeEnum.Deprecating, CardTypeEnum.Prop, CardTypeEnum.Corny}},
    {CardTypeEnum.Animals, new CardTypeEnum[] {CardTypeEnum.Romance, CardTypeEnum.Deprecating}},
    {CardTypeEnum.Romance, new CardTypeEnum[] {CardTypeEnum.Deprecating, CardTypeEnum.Dark}},
    {CardTypeEnum.Deprecating, new CardTypeEnum[] {CardTypeEnum.Family, CardTypeEnum.Animals}},
    {CardTypeEnum.Prop, new CardTypeEnum[] {CardTypeEnum.None}},
    {CardTypeEnum.Corny, new CardTypeEnum[] {CardTypeEnum.None}}};

    public Vector2 targetPosition;
    public float speed = 1.0f;
    public float size; 

    public CardTypeEnum type;
    public int maxFunny = 20;
    [SerializeField] private int curFunny = 10;

    void Start(){

        transform.localScale = Vector3.zero;
        StartCoroutine(BlipIn());
        int rand = UnityEngine.Random.Range(0, Globals.CrowdMemberTypesList.Count);
        type = Globals.CrowdMemberTypesList[rand];
        curFunny=(int)(maxFunny/2);

        transform.position = targetPosition;

        happyQuotes = new List<String>();
        sadQuotes   = new List<String>();

        happyQuotes.Add(" Ha\nHa!");
        happyQuotes.Add("Good one!");
        happyQuotes.Add("I pissed my pants!");
        happyQuotes.Add("You got that right!");
        happyQuotes.Add("You rock!");
        happyQuotes.Add("Sweeeet!");
        happyQuotes.Add("Good joke buddy!");
        happyQuotes.Add("I hear ya!");
        happyQuotes.Add("You on a roll!");

        sadQuotes.Add("Boooo!");
        sadQuotes.Add("You suck!");
        sadQuotes.Add("This guy stinks!");
        sadQuotes.Add("Get off the stage!");
        sadQuotes.Add("Worse than Amy Schumer...");
        sadQuotes.Add("Get a new joke book...");
        sadQuotes.Add("I'm 'bouta puke");
        sadQuotes.Add("*YAWN*");
        GameObject confettiObj = Instantiate(confetti.gameObject, new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z),  Quaternion.identity);
        BeNeutral();
    }

    void Update()
    { 
        // transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed );

        switch (currMood)
        {
            case Mood.Ecstatic:
                BeEcstatic();
                break;
            case Mood.Happy:
                BeHappy();
                break;
            case Mood.Neutral:
                BeNeutral();
                break;
            case Mood.Sad:
                BeSad();
                break;
            case Mood.Angry:
                BeAngry();
                break;
        }
    }

    public float GetSize()
    {
        return transform.localScale.x * transform.localScale.y;
    }

    public void TakeDamage(int funnyDamage, CardTypeEnum cardType){
        if(dislikes[type].Contains(cardType)){
            funnyDamage = ((int)(funnyDamage/2)) * -1;
        }
        else if(cardType == type){
            funnyDamage = (int)(funnyDamage*1.5f);
        }

        //sfx depending on damage?
        curFunny += funnyDamage;

        int randGen = UnityEngine.Random.Range(0, 101);

        //if we 'say' smth
        if(randGen <= 10){
            if(funnyDamage > 0){
                Debug.Log("Spawned happy text");
                int randFont = UnityEngine.Random.Range(24, 37);
                int randIndex = UnityEngine.Random.Range(0, happyQuotes.Count);
                GameObject txt = Instantiate(textPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                txt.gameObject.GetComponent<TextMeshPro>().text = happyQuotes[randIndex];
                txt.gameObject.GetComponent<TextMeshPro>().fontSize = randFont;
            }
            else if(funnyDamage < 0){
                Debug.Log("Spawned sad text");
                int randFont = UnityEngine.Random.Range(24, 37);
                int randIndex = UnityEngine.Random.Range(0, sadQuotes.Count);
                GameObject txt = Instantiate(textPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                txt.gameObject.GetComponent<TextMeshProUGUI>().text = sadQuotes[randIndex];
                txt.gameObject.GetComponent<TextMeshProUGUI>().fontSize = randFont;
            }
        }

        if(curFunny >= ecstaticThreshold){
            currMood = Mood.Ecstatic;
        }
        else if(curFunny >= happyThreshold){
            currMood = Mood.Happy;
        }
        else if(curFunny >= neutralThreshold){
            currMood = Mood.Neutral;
        }
        else if(curFunny >= sadThreshold){
            currMood = Mood.Sad;
        }
        else if(curFunny >= angryThreshold){
            currMood = Mood.Angry;
        }

        if(curFunny <= 0){
            BadLeave();
            return;
        }
        else if(curFunny >= maxFunny){
            GoodLeave();
            return;
        }
        
        float fractionResult = curFunny/maxFunny;


    }

    public void BadLeave(){
        Debug.Log("A crowd member has badly left.");
    }

    public void GoodLeave(){
        Debug.Log("A crowd member has well left.");
        GameObject confettiObj = Instantiate(confetti.gameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }


    public void BeEcstatic(){
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[0];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsWave");
        anim.speed = 2.0f;
        

    }

    public void BeHappy(){
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[1];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsWave");
        anim.speed =   1.0f;


    }

    public void BeNeutral(){
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[2];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsNeutral");
        anim.speed =   1.0f;

    }

    public void BeSad(){
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[3];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsNeutral");
        anim.speed =   1.0f;


    }

    public void BeAngry(){
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[4];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsNeutral");
        anim.speed =   1.0f;
    }

    public void Highlight(Color color)
    {
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().color = Color.white;

    }

    private IEnumerator BlipIn()
    {
        float time = 0;
        while (transform.localScale.x < 0.3f)
        {
            time += Time.deltaTime * 2;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.3f,0.3f,0.3f), time);
            yield return null;
        }
    }
}