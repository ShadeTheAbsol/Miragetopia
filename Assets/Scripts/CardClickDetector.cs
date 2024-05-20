using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardClickDetector : MonoBehaviour
{
    Animator cardAnim;
    BoxCollider2D cardCol;

    // Start is called before the first frame update
    void Start()
    {
        cardAnim = GetComponentInChildren<Animator>();
        cardCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        ShowCard();
        GameManager.instance.AssignCardsToCheck(gameObject);
    }

    public void ShowCard()
    {
        cardAnim.SetTrigger("ShowCard");
        cardCol.enabled = false;
        GameManager.instance.PlayCardFlipSound();
    }

    public void HideCard()
    {
        cardAnim.SetTrigger("HideCard");
        cardCol.enabled = true;
    }

    public void CardMatch()
    {
        cardAnim.SetTrigger("CardMatch");
        GameManager.instance.UpdateCardActiveStateList(GetComponent<CardData>().GetCardChildIndex());
    }
}
