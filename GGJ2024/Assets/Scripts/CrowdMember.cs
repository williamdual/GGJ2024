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
    public static List<String> AngrySounds      = new List<String>() {"Mad1"};
    public static List<String> SadSounds        = new List<String>() {"KindaMad1"};
    public static List<String> NeutralSounds    = new List<String>() {"Neutral1"};
    public static List<String> HappySounds      = new List<String>() {"KindaHappy1"};
    public static List<String> EcstaticSounds   = new List<String>() {"Happy1"};
    

    const int minFont = 12;
    const int maxFont = 20;
    private List<String> happyQuotes;
    private List<String> sadQuotes;

    private SoundManager soundManager;
    private GameManager gameManager;

    private float ecstaticThreshold = 0.80f;
    private float happyThreshold    = 0.60f;
    private float neutralThreshold  = 0.40f;
    private float sadThreshold      = 0.20f;
    private float angryThreshold    = 0.00f;
    [SerializeField] private Mood currMood = Mood.Neutral;

    private GameObject sadTextPrefab;
    private GameObject happyTextPrefab;
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private ParticleSystem sadParticles;

    [SerializeField] private List<Sprite> faces;
    private Dictionary<CardTypeEnum, CardTypeEnum[]> dislikes = new Dictionary<CardTypeEnum, CardTypeEnum[]>(){
    {CardTypeEnum.Family, new CardTypeEnum[] {CardTypeEnum.Dark}},
    {CardTypeEnum.Dark, new CardTypeEnum[] {CardTypeEnum.Family, CardTypeEnum.Animals, CardTypeEnum.Romance, CardTypeEnum.Deprecating, CardTypeEnum.Prop, CardTypeEnum.Corny}},
    {CardTypeEnum.Animals, new CardTypeEnum[] {CardTypeEnum.Romance, CardTypeEnum.Deprecating}},
    {CardTypeEnum.Romance, new CardTypeEnum[] {CardTypeEnum.Deprecating, CardTypeEnum.Dark}},
    {CardTypeEnum.Deprecating, new CardTypeEnum[] {CardTypeEnum.Family, CardTypeEnum.Animals}},
    {CardTypeEnum.Prop, new CardTypeEnum[] {CardTypeEnum.Corny}},
    {CardTypeEnum.Corny, new CardTypeEnum[] {CardTypeEnum.Prop}}};

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

        happyQuotes = new List<String>();
        sadQuotes   = new List<String>();

        soundManager = GameObject.FindWithTag("GameManager").gameObject.GetComponent<SoundManager>();
        gameManager  = GameObject.FindWithTag("GameManager").gameObject.GetComponent<GameManager>();

        happyQuotes.Add(" Ha\nHa!");
        happyQuotes.Add("Good one!");
        happyQuotes.Add("I pissed my pants!");
        happyQuotes.Add("You got that right!");
        happyQuotes.Add("You rock!");
        happyQuotes.Add("Sweeeet!");
        happyQuotes.Add("Good joke buddy!");
        happyQuotes.Add("I hear ya!");
        happyQuotes.Add("You on a roll!");
        happyQuotes.Add("Marry me!");
        happyQuotes.Add("The clown is funny!");
        happyQuotes.Add("HARDY! HAR! HAR!");
        happyQuotes.Add("You on a rolling wheel!");
        happyQuotes.Add("Spin me right round!");
        happyQuotes.Add("My teeth are out!");
        happyQuotes.Add("What a funny goof!");

        sadQuotes.Add("Boooo!");
        sadQuotes.Add("You suck!");
        sadQuotes.Add("This guy stinks!");
        sadQuotes.Add("Get off the stage!");
        sadQuotes.Add("Worse than Amy Schumer...");
        sadQuotes.Add("Get a new joke book...");
        sadQuotes.Add("I'm 'bouta puke");
        sadQuotes.Add("*YAWN*");
        sadQuotes.Add("I'm crying. Of Boredom.");
        sadQuotes.Add("Let's get outta here.");
        sadQuotes.Add("SO. BORING.");
        sadQuotes.Add("I'd rather die.");
        sadQuotes.Add("Screw this...");
        sadQuotes.Add("Blablabla...");
        sadQuotes.Add("Funerals are funnier.");
        
        GameObject confettiObj = Instantiate(confetti.gameObject, new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z),  Quaternion.identity);

        happyTextPrefab = (GameObject)Resources.Load("Prefabs/HappyText");
        sadTextPrefab   = (GameObject)Resources.Load("Prefabs/SadText");
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
        Debug.Log("(TYPE: " + type.ToString() + ", CARD TYPE: " + cardType.ToString() + ")\nTaking pre-damage: " + funnyDamage.ToString());
        if(dislikes[type].Contains(cardType)){
            funnyDamage = ((int)(funnyDamage)) * -1;
        }
        else if(cardType == type){
            funnyDamage = (int)(funnyDamage*1.5f);
        }
        Debug.Log("Taking post-damage: " + funnyDamage.ToString());

        //sfx depending on damage?
        curFunny += funnyDamage;

        int randGen = UnityEngine.Random.Range(0, 101);
        float minRandLife = 1.5f;
        float maxRandLife = 4.0f;

        //if we 'say' smth
        if(randGen <= 10 * ((Math.Abs(funnyDamage) * 1.7f) + 1)){
            if(funnyDamage > 0){
                Debug.Log("Spawned happy text");
                int randFont    = UnityEngine.Random.Range(minFont, maxFont);
                int randIndex   = UnityEngine.Random.Range(0, happyQuotes.Count);
                GameObject txt  = Instantiate(happyTextPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                txt.gameObject.GetComponent<TextMeshPro>().text = happyQuotes[randIndex];
                txt.gameObject.GetComponent<TextMeshPro>().fontSize = randFont;
                float randLife = UnityEngine.Random.Range(minRandLife, maxRandLife);
                txt.gameObject.GetComponent<HappyReactionText>().lifeTime = randLife;
            }
            else if(funnyDamage < 0){
                Debug.Log("Spawned sad text");
                int randFont = UnityEngine.Random.Range(minFont, maxFont);
                int randIndex = UnityEngine.Random.Range(0, sadQuotes.Count);
                GameObject txt = Instantiate(sadTextPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                txt.gameObject.GetComponent<TextMeshPro>().text = sadQuotes[randIndex];
                txt.gameObject.GetComponent<TextMeshPro>().fontSize = randFont;
                float randLife = UnityEngine.Random.Range(minRandLife, maxRandLife);
                txt.gameObject.GetComponent<SadReactionText>().lifeTime = randLife;
            }
        }

        float fracVal = (float)curFunny/(float)maxFunny;

        Debug.Log("FRACVAL: " + fracVal.ToString());

        if(fracVal >= ecstaticThreshold){
            currMood = Mood.Ecstatic;
        }
        else if(fracVal >= happyThreshold){
            currMood = Mood.Happy;
        }
        else if(fracVal >= neutralThreshold){
            currMood = Mood.Neutral;
        }
        else if(fracVal >= sadThreshold){
            currMood = Mood.Sad;
        }
        else if(fracVal >= angryThreshold){
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
    }

    public void BadLeave(){
        Debug.Log("A crowd member has badly left.");
        gameManager.TakeDamage(1);
        GameObject confettiObj = Instantiate(sadParticles.gameObject, transform.position, Quaternion.identity);
        StartCoroutine(BlipOut());
        Destroy(gameObject);
    }

    public void GoodLeave(){
        Debug.Log("A crowd member has well left.");
        gameManager.AddPeopleHappy(1);
        GameObject confettiObj = Instantiate(confetti.gameObject, transform.position, Quaternion.identity);
        StartCoroutine(BlipOut());
        Destroy(gameObject);

    }


    public void BeEcstatic(){
        int randInd = UnityEngine.Random.Range(0, EcstaticSounds.Count);
        soundManager.PlaySound(EcstaticSounds[randInd]);
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[0];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsWave");
        anim.speed = 2.0f;
        

    }

    public void BeHappy(){
        int randInd = UnityEngine.Random.Range(0, HappySounds.Count);
        soundManager.PlaySound(HappySounds[randInd]);
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[1];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsWave");
        anim.speed =   1.0f;


    }

    public void BeNeutral(){
        int randInd = UnityEngine.Random.Range(0, NeutralSounds.Count);
        soundManager.PlaySound(NeutralSounds[randInd]);
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[2];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsNeutral");
        anim.speed =   1.0f;

    }

    public void BeSad(){
        int randInd = UnityEngine.Random.Range(0, SadSounds.Count);
        soundManager.PlaySound(SadSounds[randInd]);
        GameObject body = transform.Find("Body").gameObject;
        body.GetComponent<SpriteRenderer>().sprite = faces[3];
        Animator anim = transform.Find("ArmsParent").GetComponent<Animator>();
        anim.gameObject.GetComponent<Animator>().Play("ArmsNeutral");
        anim.speed =   1.0f;


    }

    public void BeAngry(){
        int randInd = UnityEngine.Random.Range(0, AngrySounds.Count);
        soundManager.PlaySound(AngrySounds[randInd]);
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

    private IEnumerator BlipOut()
    {
        float time = 0;
        while (transform.localScale.x > 0)
        {
            time += Time.deltaTime * 2;
            transform.localScale = Vector3.Lerp(new Vector3(0.3f,0.3f,0.3f), Vector3.zero, time);
            yield return null;
        }
    }

    public void UpdatePosition(Vector2 newPosition)
    {
        Debug.Log("Updating position to: " + newPosition.ToString());
        transform.position = newPosition;
    }
}