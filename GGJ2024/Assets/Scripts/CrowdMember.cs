using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class CrowdMember : MonoBehaviour 
{
    private Dictionary<CardTypeEnum, CardTypeEnum[]> dislikes = new Dictionary<CardTypeEnum, CardTypeEnum[]>(){
    {CardTypeEnum.Family, new CardTypeEnum[] {CardTypeEnum.Dark}},
    {CardTypeEnum.Dark, new CardTypeEnum[] {CardTypeEnum.Family, CardTypeEnum.Animals, CardTypeEnum.Romance, CardTypeEnum.Depricating, CardTypeEnum.Prop, CardTypeEnum.Corny}},
    {CardTypeEnum.Animals, new CardTypeEnum[] {CardTypeEnum.Romance, CardTypeEnum.Depricating}},
    {CardTypeEnum.Romance, new CardTypeEnum[] {CardTypeEnum.Depricating, CardTypeEnum.Dark}},
    {CardTypeEnum.Depricating, new CardTypeEnum[] {CardTypeEnum.Family, CardTypeEnum.Animals}},
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
    }

    void Update()
    { 
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed );
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
}