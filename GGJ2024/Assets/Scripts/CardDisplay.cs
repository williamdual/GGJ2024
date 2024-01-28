using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public enum CardNature
    {
        Playable,
        Selectable
    }
    public CardNature cardNature;
    private TextMeshProUGUI descriptionText;

    private Image artImg;

    private TextMeshProUGUI energyText;
    private TextMeshProUGUI healthText;
    private GameObject crowdworkImg;
    
    private GameObject glow;

    private GameManager gameManager;
    private EventManager eventManager;
    private int listPos;
    private bool movingTowardsMouse = false;
    private bool playingCard = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() {
        if(movingTowardsMouse){
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0;
            gameObject.transform.position = newPos;
        }
    }
    public void setUpCardShop(EventManager em, int index)
    {
        //set actual ui objects
        descriptionText     = gameObject.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        energyText          = gameObject.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        healthText          = gameObject.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        artImg              = gameObject.transform.Find("CardArt").GetComponent<Image>();
        glow                = gameObject.transform.Find("Glow").gameObject;
        crowdworkImg        = gameObject.transform.Find("CrowdworkImg").gameObject;

        crowdworkImg.SetActive(card.isCrowdwork);

        //set visual card values
        descriptionText.text    = card.description;
        artImg.sprite           = card.artwork;
        energyText.text         = card.energyCost.ToString();
        healthText.text         = card.healthCost.ToString();
        eventManager            = em;
        listPos                 = index;
        cardNature              = CardNature.Selectable;
    }
    public void SetupFromCard(GameManager gameMan, int index){
        Debug.Log("Setting up from card: " + card.name);
        //set actual ui objects
        descriptionText     = gameObject.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        energyText          = gameObject.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        healthText          = gameObject.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        artImg              = gameObject.transform.Find("CardArt").GetComponent<Image>();
        glow                = gameObject.transform.Find("Glow").gameObject;
        crowdworkImg        = gameObject.transform.Find("CrowdworkImg").gameObject;

        crowdworkImg.SetActive(card.isCrowdwork);

        //set visual card values
        descriptionText.text    = card.description;
        artImg.sprite           = card.artwork;
        energyText.text         = card.energyCost.ToString();
        healthText.text         = card.healthCost.ToString();
        gameManager             = gameMan;
        listPos                 = index;
        cardNature              = CardNature.Playable;
    }

    public void SetListPos(int index){
        listPos = index;
    }

    private void OnMouseOver() {
        glow.SetActive(true);
    }

    private void OnMouseEnter() {
        gameObject.GetComponent<Canvas>().overrideSorting = true;
        gameObject.GetComponent<Canvas>().sortingOrder = 5;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + 0.005f, gameObject.transform.localScale.y + 0.005f, gameObject.transform.localScale.z + 0.005f);
    }

    private void OnMouseDown() {
        if(cardNature == CardNature.Playable){
            gameManager.SetCardCanMove(false, listPos);
            movingTowardsMouse = true;
        }
    }

    private void OnMouseUp() {
        movingTowardsMouse = false;
        if(playingCard && cardNature == CardNature.Playable){
            gameManager.SetCardCanMove(true, listPos);
            gameManager.PlayCard(listPos);
        }
        else if(playingCard && cardNature == CardNature.Selectable){
            eventManager.selectCard(listPos);
        }
    }

    private void OnMouseExit() {
        glow.SetActive(false);
        if(cardNature == CardNature.Playable)
        {
            gameManager.SetCardCanMove(true, listPos);
        }
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
        gameObject.GetComponent<Canvas>().overrideSorting = false;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - 0.005f, gameObject.transform.localScale.y - 0.005f, gameObject.transform.localScale.z - 0.005f);
    }

    public AudioClip ReturnAudioClip(){
        return card.audioClip;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("PlayArea")){
            playingCard = true;
            if (gameManager.GetCanPlay())
            {
                GameObject.FindWithTag("Floor").GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("PlayArea")){
            playingCard = false;
            GameObject.FindWithTag("Floor").GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
