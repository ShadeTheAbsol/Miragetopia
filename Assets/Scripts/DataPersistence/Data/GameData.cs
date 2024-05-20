using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int columns;
    public int rows;

    public List<int> shuffledCardIndexes;
    public List<bool> cardActiveStates;
    public bool gameWon;

    public int cardMatches;
    public int score;
    public int scoreMultiplier;

    public GameData()
    {
        columns = 0;
        rows = 0;
        shuffledCardIndexes = new List<int>();
        cardActiveStates = new List<bool>();
        gameWon = false;

        cardMatches = 0;
        score = 0;
        scoreMultiplier = 0;
    }
}
