using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Transform cardsContainer;
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
    }
}
