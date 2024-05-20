using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    private Card data;

    private string cardTitle;
    private Sprite cardSprite;
    private int childIndex;
    private bool cardActive;

    // Start is called before the first frame update
    void Start()
    {
        cardActive = true;
        childIndex = transform.GetSiblingIndex();
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
        transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = cardSprite;
    }

    public string GetCardName()
    {
        return cardTitle;
    }

    public Sprite GetCardArt()
    {
        return cardSprite;
    }

    public int GetCardChildIndex()
    {
        return childIndex;
    }

    public void DeactivateCard()
    {
        cardActive = false;
    }

    public void SetCardActiveState(bool state)
    {
        cardActive = state;
    }

    public bool GetCardActiveState()
    {
        return cardActive;
    }

    public Card GetCardData()
    {
        return data;
    }
}
