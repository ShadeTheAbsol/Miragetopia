using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Transform cardsContainer;
    public Card[] cardTypes;
    public TMP_InputField gridColumnsInput;
    public TMP_InputField gridRowsInput;
    public TextMeshProUGUI errorText;

    private CardGrid cardOrganizer;
    private List<GameObject> cards;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        cardOrganizer = cardsContainer.GetComponent<CardGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        if (string.IsNullOrEmpty(gridColumnsInput.text.Trim()) || string.IsNullOrEmpty(gridRowsInput.text.Trim()))
        {
            errorText.text = "Please Enter Valid Grid Size";
            return;
        }

        int columns = int.Parse(gridColumnsInput.text);
        int rows = int.Parse(gridRowsInput.text);

        if (columns * rows % 2 == 0)
        {
            if (columns * rows > 30)
            {
                errorText.text = "Please Enter A Smaller Grid";
                return;
            }

            errorText.text = "";
            cardOrganizer.SetupCardGrid(columns, rows);
            SetupGame();
        }
        else
        {
            errorText.text = "Please Enter Valid Grid Size";
        }
    }

    private void SetupGame()
    {
        cards = new List<GameObject>();

        for (int i = 0; i < cardsContainer.childCount; i++)
        {
            cards.Add(cardsContainer.GetChild(i).gameObject);
        }

        SetupCards();
    }

    private void SetupCards()
    {
        int typesNeeded = cards.Count / 2;

        List<Card> cardTypesSelected = new List<Card>(GetRandomCardTypes(cardTypes, typesNeeded));

        int cardIndex = 0;

        for (int  i = 0; i < cardTypesSelected.Count; i++)
        {
            cards[cardIndex].GetComponent<CardData>().AssignCardData(cardTypesSelected[i]);
            cards[cardIndex+1].GetComponent<CardData>().AssignCardData(cardTypesSelected[i]);
            cardIndex += 2;
        }

        ShuffleCards(cards);
    }

    private List<Card> GetRandomCardTypes(Card[] cardTypes, int numberOfTypes)
    {
        // Create a copy of cardTypes list
        List<Card> cardTypesTempList = new List<Card>(cardTypes);

        // Shuffle the list
        for (int i = cardTypesTempList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = cardTypesTempList[i];
            cardTypesTempList[i] = cardTypesTempList[randomIndex];
            cardTypesTempList[randomIndex] = temp;
        }

        // Select number of types from shuffled type list
        List<Card> selectedCardTypes = cardTypesTempList.GetRange(0, numberOfTypes);

        return selectedCardTypes;
    }

    private void ShuffleCards(List<GameObject> passedCardList)
    {
        // Shuffle cards
        for (int i = passedCardList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = passedCardList[i].GetComponent<CardData>().GetCardData();
            passedCardList[i].GetComponent<CardData>().AssignCardData(passedCardList[randomIndex].GetComponent<CardData>().GetCardData());
            passedCardList[randomIndex].GetComponent<CardData>().AssignCardData(temp);
        }
    }

    public void Reshuffle()
    {
        ShuffleCards(cards);
    }
}
