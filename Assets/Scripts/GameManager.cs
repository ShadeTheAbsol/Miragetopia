using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Transform cardsContainer;
    public Card[] cardTypes;
    public TMP_InputField gridColumnsInput;
    public TMP_InputField gridRowsInput;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreMultiplierText;
    public int scorePerMatch;

    public GameObject mainMenuScreen;
    public GameObject PauseScreen;
    public GameObject GameOverScreen;
    public TextMeshProUGUI finalScoreText;

    public AudioMixer masterMixer;
    public AudioClip cardFlipSFX;
    public AudioClip cardMatchCorrectSFX;
    public AudioClip cardMatchFailSFX;
    public AudioClip gameOverSFX;


    private CardGrid cardOrganizer;
    private List<GameObject> cards;
    //Cards used to check for matching cards
    private GameObject cardOne;
    private GameObject cardTwo;
    private int cardMatches;
    private int score;
    private int scoreMultiplier;
    private AudioSource SFXPlayer;

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
        cardMatches = 0;
        score = 0;
        scoreMultiplier = 1;
        SFXPlayer = GetComponent<AudioSource>();
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
            mainMenuScreen.SetActive(false);
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

    public void AssignCardsToCheck(GameObject passedCard)
    {
        if (cardOne == null)
        {
            cardOne = passedCard;
            return;
        }

        if (cardTwo == null)
        {
            cardTwo = passedCard;

            Card cardOneData = cardOne.GetComponent<CardData>().GetCardData();
            Card cardTwoData = cardTwo.GetComponent<CardData>().GetCardData();

            if (CheckCardsForMatch(cardOneData, cardTwoData))
            {
                cardOne.GetComponent<CardClickDetector>().CardMatch();
                cardTwo.GetComponent<CardClickDetector>().CardMatch();
                cardMatches++;
                PlayCardMatchCorrectSound();
                score += scorePerMatch * scoreMultiplier;
                scoreText.text = score.ToString();

                if (cardMatches == cards.Count / 2)
                    GameOver();

                scoreMultiplier++;
                scoreMultiplierText.text = scoreMultiplier.ToString();
            }
            else
            {
                cardOne.GetComponent<CardClickDetector>().HideCard();
                cardTwo.GetComponent<CardClickDetector>().HideCard();
                PlayCardMatchFailSound();
                scoreMultiplier = 1;
                scoreMultiplierText.text = scoreMultiplier.ToString();
            }

            cardOne = null;
            cardTwo = null;
        }
    }

    private bool CheckCardsForMatch(Card cardOneData, Card cardTwoData)
    {
        if (string.Equals(cardOneData.name, cardTwoData.name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GameOver()
    {
        PlayGameOverSound();
        finalScoreText.text = score.ToString();
        GameOverScreen.SetActive(true);
    }

    public void PauseGame()
    {
        PauseScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayCardFlipSound()
    {
        masterMixer.SetFloat("SFXVolume", 10);
        SFXPlayer.PlayOneShot(cardFlipSFX);
    }

    public void PlayCardMatchCorrectSound()
    {
        masterMixer.SetFloat("SFXVolume", 10);
        SFXPlayer.PlayOneShot(cardMatchCorrectSFX);
    }

    public void PlayCardMatchFailSound()
    {
        masterMixer.SetFloat("SFXVolume", 10);
        SFXPlayer.PlayOneShot(cardMatchFailSFX);
    }

    public void PlayGameOverSound()
    {
        masterMixer.SetFloat("SFXVolume", 10);
        SFXPlayer.PlayOneShot(gameOverSFX);
    }
}
