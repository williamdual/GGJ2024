using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    private TextMeshProUGUI descriptionText;

    private Image artImg;

    private TextMeshProUGUI energyText;
    private TextMeshProUGUI healthText;
    private GameObject crowdworkImg;
    
    private GameObject glow;

    private GameManager gameManager;
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
    }

    public void SetListPos(int index){
        listPos = index;
    }

    private void OnMouseOver() {
        glow.SetActive(true);
    }

    private void OnMouseDown() {
        gameManager.SetCardCanMove(false, listPos);
        movingTowardsMouse = true;
    }

    private void OnMouseUp() {
        gameManager.SetCardCanMove(true, listPos);
        movingTowardsMouse = false;
        if(playingCard){
            gameManager.PlayCard(listPos);
        }
    }

    private void OnMouseExit() {
        glow.SetActive(false);
        gameManager.SetCardCanMove(true, listPos);
    }

    public AudioClip ReturnAudioClip(){
        return card.audioClip;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("PlayArea")){
            playingCard = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("PlayArea")){
            playingCard = false;
        }
    }
}
