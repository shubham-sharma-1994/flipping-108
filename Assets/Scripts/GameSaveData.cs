using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int columns;
    public int rows;
    public int matches;
    public int turns;
    public int totalPairs;

    public List<CardSaveData> cards = new List<CardSaveData>();
    public bool IsInProgress => matches < totalPairs;
}


[Serializable]
public class CardSaveData
{
    public int  cardId;
    public bool isMatched;
}
