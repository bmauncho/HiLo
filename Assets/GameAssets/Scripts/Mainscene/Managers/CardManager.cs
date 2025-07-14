using UnityEngine;
public enum CardSuites
{
    CLUB,
    DIAMOND,
    HEART,
    SPADE,
}

public enum CardColor
{
    BLACK,
    RED,
}

public enum CardRanks
{
    ACE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    SEVEN,
    EIGHT,
    NINE,
    TEN,
    JACK,
    QUEEN,
    KING,
}

[System.Serializable]
public class Suites
{
    public CardSuites suite;
    public SuitesConfig [] suitesConfigs;
}

[System.Serializable]
public class  SuitesConfig
{
    public CardColor color;
    public Sprite sprite;
}

[System.Serializable]
public class Ranks
{
    public CardRanks rank;
    public RanksConfig [] ranksConfigs;
}

[System.Serializable]
public class RanksConfig
{
    public CardColor color;
    public Sprite sprite;
}

public class CardManager : MonoBehaviour
{
    public Suites [] suitesList;
    public Ranks [] ranksList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
