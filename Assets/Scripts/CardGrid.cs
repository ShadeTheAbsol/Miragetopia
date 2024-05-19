using UnityEngine;

public class CardGrid : MonoBehaviour
{
    public GameObject cardPrefab;
    public int columns; // Card Columns
    public int rows; // Card Rows
    public float screenOffset; // Offset between edge of the screen and cards
    public float cardGap; // Gap between cards

    void Start()
    {
        SetupCardGrid();
    }

    private void SetupCardGrid()
    {
        // Get screen width and screen height
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        // Calculate available width and height for card grid but excluding the offset.
        float availableWidth = screenWidth - 2 * screenOffset;
        float availableHeight = screenHeight - 2 * screenOffset;

        // Determine Amount of columns and rows that can fit on screen based on card size
        int maxColumns = Mathf.FloorToInt((availableWidth + cardGap) / (cardPrefab.transform.localScale.x + cardGap));
        int maxRows = Mathf.FloorToInt((availableHeight + cardGap) / (cardPrefab.transform.localScale.y + cardGap));

        //Clamp columns and rows so they do not go beyond max that can fit on screen
        columns = Mathf.Clamp(columns, 1, maxColumns);
        rows = Mathf.Clamp(rows, 1, maxRows);

        // Calculate card size based on available width but excluding gaps between the cards.
        float cardWidth = (availableWidth - (columns - 1) * cardGap) / columns;
        float cardHeight = (availableHeight - (rows - 1) * cardGap) / rows;

        // Calculate card starting position so grid starts in the top-left corner, screenOffset and card size also accounted for
        Vector3 cardStartPos = new Vector3(-screenWidth / 2 + screenOffset + cardWidth / 2,screenHeight / 2 - screenOffset - cardHeight / 2,0
        );

        // Create The Card Grid
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Calculate current card position using card size and gap
                float currentCardXPos = cardStartPos.x + column * (cardWidth + cardGap);
                float currentCardYPos = cardStartPos.y - row * (cardHeight + cardGap);

                // Create current card at position
                Vector3 newCardPos = new Vector3(currentCardXPos, currentCardYPos, 0f);
                GameObject card = Instantiate(cardPrefab, newCardPos, Quaternion.identity, transform);
                card.name = "Card";

                //Scales Card To Sprite Size
                float scaleX = Mathf.Min(cardWidth / cardPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.x, 1);
                float scaleY = Mathf.Min(cardHeight / cardPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.y, 1);
                card.transform.localScale = new Vector3(scaleX, scaleY, 1);
            }
        }
    }
}