using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour, IDataPersistence
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
    private int columns;
    private int rows;
    private List<GameObject> cards;
    private List<int> shuffledCardTypeIndexes;
    private List<bool> cardActiveStates;
    //Cards used to check for matching cards
    private GameObject cardOne;
    private GameObject cardTwo;
    private int cardMatches;
    private int score;
    private int scoreMultiplier;
    private bool gameWon;

    private AudioSource SFXPlayer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        cardOrganizer = cardsContainer.GetComponent<CardGrid>();
        cardActiveStates = new List<bool>();
        scoreMultiplier = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        
        columns = int.Parse(gridColumnsInput.text);
        rows = int.Parse(gridRowsInput.text);

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
        SetupShuffledCardIndexes();

        for (int i = 0; i < cards.Count; i++)
        {
            cardActiveStates.Add(true);
        }
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

    private void SetupShuffledCardIndexes()
    {
        shuffledCardTypeIndexes = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            for (int j = 0; j < cardTypes.Length; j++)
            {
                if (string.Equals(cards[i].GetComponent<CardData>().GetCardName(), cardTypes[j].name))
                {
                    shuffledCardTypeIndexes.Add(j);
                    break;
                }
            }
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
                cardOne.GetComponent<CardData>().DeactivateCard();
                cardTwo.GetComponent<CardData>().DeactivateCard();
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
        gameWon = true;
        PlayGameOverSound();
        finalScoreText.text = score.ToString();
        GameOverScreen.SetActive(true);
    }

    public void UpdateCardActiveStateList(int cardIndex)
    {
        cardActiveStates[cardIndex] = false;
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

    public void LoadData(GameData data)
    {
        gameWon = data.gameWon;
        columns = data.columns;
        rows = data.rows;

        if (gameWon || columns == 0 || rows == 0)
        {
            mainMenuScreen.SetActive(true);
            gameWon = false;
            columns = 0;
            rows = 0;
            return;
        }

        shuffledCardTypeIndexes = data.shuffledCardIndexes;
        cardActiveStates = data.cardActiveStates;
        cardMatches = data.cardMatches;
        score = data.score;
        scoreMultiplier = data.scoreMultiplier;

        scoreText.text = score.ToString();
        scoreMultiplierText.text = scoreMultiplier.ToString();

        cardOrganizer.SetupCardGrid(columns, rows);
        mainMenuScreen.SetActive(false);

        cards = new List<GameObject>();

        for (int i = 0; i < cardsContainer.childCount; i++)
        {
            cards.Add(cardsContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<CardData>().AssignCardData(cardTypes[data.shuffledCardIndexes[i]]);

            if (cardActiveStates[i] == false)
                cards[i].SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        data.columns = this.columns;
        data.rows = this.rows;
        data.shuffledCardIndexes = shuffledCardTypeIndexes;
        data.cardActiveStates = cardActiveStates;
        data.gameWon = gameWon;
        data.cardMatches = cardMatches;
        data.score = score;
        data.scoreMultiplier = scoreMultiplier;
    }
}
