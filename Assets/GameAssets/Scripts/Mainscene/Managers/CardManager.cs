using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public enum CardSuites
{
    CLUBS,
    DIAMONDS,
    HEARTS,
    SPADES,
}

public enum CardColor
{
    BLACK,
    RED,
}

public enum CardRanks
{
    ACE = 0,
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
public class faceRansks
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

[System.Serializable]
public class SuiteWeight
{
    public CardSuites cardSuite;
    [UnityEngine.Range(1 , 20)] public float weight;
    public SuiteWeight ( CardSuites type , float w )
    {
        cardSuite = type;
        weight = w;
    }
}

[System.Serializable]
public class ColorWeight
{
    public CardColor color;
    [UnityEngine.Range(1 , 20)] public float weight;
    public ColorWeight ( CardColor c , float w )
    {
        color = c;
        weight = w;
    }
}

[System.Serializable]
public class CardWeight
{
    public CardRanks cardRank;
    [UnityEngine.Range(1 , 20)] public float weight;

    public CardWeight ( CardRanks type , float w )
    {
        cardRank = type;
        weight = w;
    }
}

[System.Serializable]
public class CardData
{
    public CardSuites cardSuite;
    public CardColor cardColor;
    public CardRanks cardRank;
}

public class CardManager : MonoBehaviour
{
    GamePlayManager gamePlayManager;
    ApiManager apiManager;
    [Header("Card Suites and Ranks")]
    public Suites [] suitesList;
    public Ranks [] ranksList;
    public faceRansks [] faceRanksList;

    [Header("Suite & Card Type Weights")]
    private float totalCardWeight = 0f; // Total weight of all card types
    private float totalSuiteWeight = 0f; // Total weight of all suites 
    private float totalColorWeight = 0f; // Total weight of all card weights

    [SerializeField] private List<ColorWeight> colorWeights = new List<ColorWeight>();
    [SerializeField] private List<SuiteWeight> suiteWeights = new List<SuiteWeight>();
    [SerializeField] private List<CardWeight> cardWeights = new List<CardWeight>();
    [Header("Card Data")]
    [SerializeField] private CardData prevCard;
    [SerializeField] private CardData currentCard;
    bool init = false;
    private void Awake ()
    {
        InitializeDefaultSuiteWeights();
        InitializeDefaultColorWeights();
        InitializeDefaultRanksWeights();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
        apiManager = CommandCenter.Instance.apiManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region[Sprite Data]
    public Sprite GetCardSuiteSprite ( CardSuites suite , CardColor color )
    {
        foreach (var s in suitesList)
        {
            if (s.suite == suite)
            {
                foreach (var config in s.suitesConfigs)
                {
                    if (config.color == color)
                    {
                        return config.sprite;
                    }
                }
            }
        }
        return null;
    }

    public Sprite GetCardRankSprite ( CardRanks rank , CardColor color )
    {
        foreach (var r in ranksList)
        {
            if (r.rank == rank)
            {
                foreach (var config in r.ranksConfigs)
                {
                    if (config.color == color)
                    {
                        return config.sprite;
                    }
                }
            }
        }
        return null;
    }

    public CardColor GetCardColor ( CardSuites suite )
    {
        foreach (var s in suitesList)
        {
            if (s.suite == suite)
            {
                return s.suitesConfigs [0].color; // Assuming all configs for a suite have the same color
            }
        }
        return CardColor.BLACK; // Default color if not found
    }

    public Sprite GetCardFaceRankSprite ( CardRanks rank , CardColor color )
    {
        foreach (var r in faceRanksList)
        {
            if (r.rank == rank)
            {
                foreach (var config in r.ranksConfigs)
                {
                    if (config.color == color)
                    {
                        return config.sprite;
                    }
                }
            }
        }
        return null;
    }

    #endregion

    private void InitializeDefaultRanksWeights ()
    {
        if (cardWeights.Count == 0)
        {
            cardWeights.Add(new CardWeight(CardRanks.ACE , 7f));
            cardWeights.Add(new CardWeight(CardRanks.TWO , 7f));
            cardWeights.Add(new CardWeight(CardRanks.THREE , 7f));
            cardWeights.Add(new CardWeight(CardRanks.FOUR , 7f));
            cardWeights.Add(new CardWeight(CardRanks.FIVE , 7f));
            cardWeights.Add(new CardWeight(CardRanks.SIX , 7f));
            cardWeights.Add(new CardWeight(CardRanks.SEVEN , 7f));
            cardWeights.Add(new CardWeight(CardRanks.EIGHT , 7f));
            cardWeights.Add(new CardWeight(CardRanks.NINE , 7f));
            cardWeights.Add(new CardWeight(CardRanks.TEN , 7f));
            cardWeights.Add(new CardWeight(CardRanks.JACK , 7f));
            cardWeights.Add(new CardWeight(CardRanks.QUEEN , 7f));
            cardWeights.Add(new CardWeight(CardRanks.KING , 7f));
        }

        totalCardWeight = cardWeights.Sum(weight => weight.weight); // Precompute total weights
    }

    private void InitializeDefaultSuiteWeights ()
    {
        if (suiteWeights.Count == 0)
        {
            suiteWeights.Add(new SuiteWeight(CardSuites.CLUBS , 5f));
            suiteWeights.Add(new SuiteWeight(CardSuites.DIAMONDS , 5f));
            suiteWeights.Add(new SuiteWeight(CardSuites.HEARTS , 5f));
            suiteWeights.Add(new SuiteWeight(CardSuites.SPADES , 5f));
        }

        totalSuiteWeight = suiteWeights.Sum(weight => weight.weight); // Precompute total weights
    }

    private void InitializeDefaultColorWeights ()
    {
        if (colorWeights.Count == 0)
        {
            colorWeights.Add(new ColorWeight(CardColor.BLACK , 5f));
            colorWeights.Add(new ColorWeight(CardColor.RED , 5f));
        }

        totalColorWeight = colorWeights.Sum(weight => weight.weight); // Precompute total weights
    }

    public CardSuites GetRandomCardSuite ()
    {
        float randomValue = Random.Range(0f , totalSuiteWeight);
        float cumulativeWeight = 0f;
        foreach (var weight in suiteWeights)
        {
            cumulativeWeight += weight.weight;
            if (randomValue <= cumulativeWeight)
            {
                return weight.cardSuite; // Return the corresponding card suite
            }
        }
        return CardSuites.CLUBS; // Default return if no match found
    }

    public CardColor GetRandomCardColor ( CardSuites suite )
    {
        float randomValue = Random.Range(0f , totalColorWeight);
        float cumulativeWeight = 0f;
        foreach (var weight in colorWeights)
        {
            if (weight.color == GetCardColor(suite)) // Match the suite's color
            {
                cumulativeWeight += weight.weight;
                if (randomValue <= cumulativeWeight)
                {
                    return weight.color; // Return the corresponding card color
                }
            }
        }
        return CardColor.BLACK; // Default return if no match found
    }

    public CardRanks GetRandomCardRank ()
    {
        float randomValue = Random.Range(0f , totalCardWeight);
        float cumulativeWeight = 0f;
        foreach (var weight in cardWeights)
        {
            cumulativeWeight += weight.weight;
            if (randomValue <= cumulativeWeight)
            {
                return weight.cardRank; // Return the corresponding card rank
            }
        }
        return CardRanks.ACE; // Default return if no match found
    }

    public CardData GetCardData ()
    {
        prevCard = currentCard;
        CardData data = new CardData();
       
        if (CommandCenter.Instance.IsDemo())
        {
            data.cardSuite = GetRandomCardSuite();
            data.cardColor = GetRandomCardColor(data.cardSuite);
            data.cardRank = GetRandomCardRank();
        }
        else
        {
            CardData newData = GetCardDataFromApi();
            data.cardSuite = newData.cardSuite;
            data.cardColor = GetRandomCardColor(data.cardSuite);
            data.cardRank = newData.cardRank;
        }

        if (!isColorAvailable(data.cardSuite , data.cardColor))
        {
            Debug.LogWarning($"Color {data.cardColor} is not available for suite {data.cardSuite}");
            // if black color is not available, fallback to red or a default color
            // if red color is not available, fallback to black or a default color
            if (data.cardColor == CardColor.BLACK)
            {
                data.cardColor = CardColor.RED; // Fallback to red if black is not available
            }
            else
            {
                data.cardColor = CardColor.BLACK; // Fallback to black if red is not available
            }
        }

        //Debug.Log($"Generated Card Data: Suite={data.cardSuite}, Color={data.cardColor}, Rank={data.cardRank}");
        currentCard = data;
        return data;
    }

    public CardData GetCurrentCardData ()
    {
        //Debug.Log($"GetCurrentCardData: returning {currentCard.cardRank} of {currentCard.cardSuite} ({currentCard.cardColor})");
        return currentCard;
    }


    public CardData GetPrevCardData ()
    {
        return prevCard;
    }

    public bool isColorAvailable(CardSuites cardSuites,CardColor color )
    {
        foreach(var suite in suitesList)
        {
            if (suite.suite == cardSuites)
            {
                foreach (var config in suite.suitesConfigs)
                {
                    if (config.color == color)
                    {
                        return true; // Color is available for the given suite
                    }
                }
            }
        }
        return false; // Color not found for the given suite
    }

    public void ResetCardData ()
    {
        //when we lose
        currentCard = prevCard;
    }
    //live
    public CardData GetCardDataFromApi ()
    {
        CardData data = new CardData();
        string currentCard = "";

        bool IsFirstTime = gamePlayManager.Get_IsFirstTime();
        bool IsSkip = gamePlayManager.Get_IsSkip();
        bool IsGameStarted = gamePlayManager.IsGameStarted();

        switch (IsSkip)
        {
            case true:
                switch (IsGameStarted)
                {
                    case true:
                        currentCard = apiManager.SkipApi.skipResponse.game_state.current_card;
                        break;
                    case false:
                        currentCard = apiManager.previewSkipApi.response.current_card;
                        break;
                }
                break;
            case false:
                switch (init)
                {
                    case true:
                        currentCard = apiManager.guessApi.guessResponse.game_state.current_card;
                       // Debug.Log(currentCard);
                        break;
                    case false:
                        currentCard = $"{GameManager.Instance.previewApi.response.current_card}";
                        init = true;
                       // Debug.Log(currentCard);
                        break;
                }
               
                break;
        }

        //Debug.Log(currentCard);
        CardData newData = ParseCard(currentCard);
        data = new CardData
        {
            cardRank = newData.cardRank ,
            cardSuite = newData.cardSuite,
        };
        return data;
    }

    public CardData ParseCard ( string currentCard )
    {
        if (string.IsNullOrWhiteSpace(currentCard))
            throw new ArgumentException("Invalid card format: <null or empty>");

        string [] parts = currentCard.Split('_');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid card format: " + currentCard);

        string part1 = parts [0];
        string part2 = parts [1];

        bool part1IsSuit = Enum.TryParse<CardSuites>(part1 , true , out var suit1);
        bool part2IsRank = Enum.TryParse<CardRanks>(part2 , true , out var rank1);

        if (part1IsSuit && part2IsRank)
        {
            return new CardData { cardSuite = suit1 , cardRank = rank1 };
        }

        // Try reversed
        bool part2IsSuit = Enum.TryParse<CardSuites>(part2 , true , out var suit2);
        bool part1IsRank = Enum.TryParse<CardRanks>(part1 , true , out var rank2);

        if (part2IsSuit && part1IsRank)
        {
            return new CardData { cardSuite = suit2 , cardRank = rank2 };
        }

        throw new ArgumentException($"Invalid card format or enums do not match: {currentCard}");
    }

}