using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    private Card data;

    private string cardTitle;
    private Sprite cardSprite;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AssignCardData(Card cardData)
    {
        data = cardData;
        SetupCard();
    }

    private void SetupCard()
    {
        cardTitle = data.name;
        cardSprite = data.art;

        transform.name = cardTitle + " Card";
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = cardSprite;
    }

    public string GetCardName()
    {
        return cardTitle;
    }

    public Sprite GetCardArt()
    {
        return cardSprite;
    }

    public Card GetCardData()
    {
        return data;
    }
}
