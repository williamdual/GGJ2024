using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Mood currMood = Mood.Neutral;
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

    public float health; 

    public CardTypeEnum type;
    public int maxFunny = 20;
    private int curFunny = 10;

    void Start(){
        curFunny=(int)(maxFunny/2);
        GameObject confettiObj = Instantiate(confetti.gameObject, transform.position, Quaternion.identity);

        // BeNeutral();
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
        if(curFunny <= 0){
            BadLeave();
        }
        else if(curFunny >= maxFunny){
            GoodLeave();
        }
    }

    public void BadLeave(){
        Debug.Log("A crowd member has badly left.");
    }

    public void GoodLeave(){
        Debug.Log("A crowd member has well left.");
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
}