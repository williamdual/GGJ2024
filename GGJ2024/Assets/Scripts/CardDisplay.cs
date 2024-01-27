using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI descriptionText;

    private Image artImg;

    private TextMeshProUGUI energyText;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI funnyText;
    private TextMeshProUGUI typeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupFromCard(){
        Debug.Log("Setting up from card: " + card.name);
        //set actual ui objects
        nameText        = gameObject.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        descriptionText = gameObject.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        energyText      = gameObject.transform.Find("EnergyImg/EnergyText").GetComponent<TextMeshProUGUI>();
        healthText      = gameObject.transform.Find("HealthImg/HealthText").GetComponent<TextMeshProUGUI>();
        funnyText       = gameObject.transform.Find("FunnyImg/FunnyText").GetComponent<TextMeshProUGUI>();
        typeText        = gameObject.transform.Find("TypeImg/TypeText").GetComponent<TextMeshProUGUI>();
        artImg          = gameObject.transform.Find("CardArt").GetComponent<Image>();

        //set visual card values
        nameText.text           = card.cardName;
        descriptionText.text    = card.description;
        artImg.sprite           = card.artwork;
        energyText.text         = card.energyCost.ToString();
        healthText.text         = card.healthCost.ToString();
        funnyText.text          = card.funnyValue.ToString();
        typeText.text           = card.cardType.ToString();
    }
}
