using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CardHistoryData
{
    public CardData cardData;
    public MultiplierType multiplierType;
    public OutCome outCome;
}

public enum OutlineColor
{
    Red,
    Yellow,
}
[System.Serializable]
public class Outline
{
    public OutCome TheOutCome;
    public Sprite outline;
}


[System.Serializable]
public class BetHistory
{
    public MultiplierType multiplierType;
    public Sprite betHistory;
}

[System.Serializable]
public class BetHistoryCardData
{
    public Sprite cardRank;
    public Sprite cardSuite_Icon;
    public Sprite CardSuite_Bg;
}

public class CardHistory : MonoBehaviour
{
    WinLoseManager winloseManager;
    CardManager cardManager;
    PoolManager poolManager;
    MultiplierManager multiplierManager;
    GamePlayManager gamePlayManager;
    public int HistoryDataIndex = -1;
    public Transform HistoryContent;
    public Sprite skip;
    public Outline [] OutlineData;
    public BetHistory [] BetHistoryData;
    public List<CardHistoryData> HistoryData = new List<CardHistoryData>();
    List<GameObject> historyCards = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winloseManager = CommandCenter.Instance.winLoseManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        poolManager = CommandCenter.Instance.poolManager_;
        multiplierManager = CommandCenter.Instance.multiplierManager_;
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHistoryData ()
    {
        ClearHistory();
    }

    public void AddHistoryData (
         CardData cardData ,
         MultiplierType multiplier ,
         OutCome outCome )
    {
        HistoryData.Add(
            new CardHistoryData
            {
                cardData = new CardData
                {
                    cardSuite = cardData.cardSuite ,
                    cardColor = cardData.cardColor ,
                    cardRank = cardData.cardRank ,
                } ,
                multiplierType = multiplier ,
                outCome = outCome
            }
        );
        HistoryDataIndex++;
    }

    public Sprite GetOutline (OutCome TheOutCome,bool isSkip)
    {
        if (isSkip)
        {
            return OutlineData [0].outline;
        }
        foreach(Outline outline in OutlineData)
        {
            if(outline.TheOutCome == TheOutCome)
            {
                return outline.outline;
            }
        }
        
        return OutlineData [0].outline;
    }

    public Sprite GetBetHistoryConfig (MultiplierType multiplierType,bool IsSkip)
    {
        if (IsSkip)
        {
            return GetSkip();
        }

        foreach(BetHistory betHistory in BetHistoryData)
        {
            if(betHistory.multiplierType == multiplierType)
            {
                return betHistory.betHistory;
            }
        }
        return null;
    }

    public Sprite GetSkip ()
    {
        return skip;
    }

    public BetHistoryCardData GetHistoryCardData ( 
        CardSuites suite , 
        CardRanks rank , 
        CardColor color )
    {
        BetHistoryCardData betHistoryCardData = new BetHistoryCardData();
        bool isfaceCard = IsFaceCard(rank);
        Sprite Rank = cardManager.GetCardRankSprite(rank , color);
        Sprite Suite_Icon = cardManager.GetCardSuiteSprite(suite , color);
        Sprite Suite_Bg = isfaceCard? cardManager.GetCardFaceRankSprite(rank , color) : cardManager.GetCardSuiteSprite(suite , color);

        betHistoryCardData = new BetHistoryCardData
        {
            cardRank = Rank ,
            cardSuite_Icon = Suite_Icon ,
            CardSuite_Bg = Suite_Bg ,
        };
        return betHistoryCardData;
    }


    public void ShowHistory ()
    {
        //Debug.Log(HistoryDataIndex);
        Debug.Log($"{HistoryData [HistoryDataIndex].cardData.cardSuite} \n" +
            $"{HistoryData [HistoryDataIndex].cardData.cardColor} \n" +
            $"{HistoryData [HistoryDataIndex].cardData.cardRank} \n" +
            $"{HistoryData [HistoryDataIndex].multiplierType} \n" +
            $"{HistoryData [HistoryDataIndex].outCome} \n");

        bool canshowHistory = !gamePlayManager.Skips.IsFirstTime();
        bool IsSkip = gamePlayManager.Skips.IsFirstTime() ? 
            false : isSkipMode(gamePlayManager.Skips.GetSkipMode());

        OutCome theOutcome = winloseManager.GetTheOutCome();
        CardData prevCardData = cardManager.GetPrevCardData();
        CardData currCardData = cardManager.GetCurrentCardData();

        MultiplierType multiplierType = multiplierManager.selectedMultiplier;
        MultiplierType newMultiplierType = 
            gamePlayManager.whichSelectedMultiplier(
                prevCardData , 
                currCardData , 
                multiplierType);

        Debug.Log($"multipler type : {newMultiplierType.ToString()}");

        BetHistoryCardData betHistoryCardData = new BetHistoryCardData();
        CardData cardData = cardManager.GetCurrentCardData();
        betHistoryCardData = GetHistoryCardData(
            cardData.cardSuite,
            cardData.cardRank,
            cardData.cardColor);

        //Get from pool 
        GameObject historyCard = poolManager.GetFromPool(
            PoolType.CardHistory,
            Vector3.zero,
            Quaternion.identity,
            HistoryContent);

        //set up Data

        if (historyCard != null)
        {
            History historyComponenet = historyCard.GetComponent<History>();
            if (historyComponenet != null)
            {
                historyComponenet.SetCardHistory(
                    GetOutline(theOutcome,IsSkip) ,
                    betHistoryCardData.cardRank,
                    betHistoryCardData.cardSuite_Icon,
                    betHistoryCardData.CardSuite_Bg,
                    GetBetHistoryConfig(newMultiplierType , IsSkip),
                    canshowHistory);
            }
        }

        historyCards.Add( historyCard );
    }

    public bool IsFaceCard ( CardRanks cardRank )
    {
        return cardRank == CardRanks.JACK ||
               cardRank == CardRanks.QUEEN ||
               cardRank == CardRanks.KING;
    }


    public void ClearHistory ()
    {
        HistoryData.Clear();
        StartCoroutine(returnToPool());
        Debug.Log("Return history cards to pool!");
    }

    IEnumerator returnToPool ()
    {
        if (historyCards.Count <= 0) yield break;
        int cardsToReturn = historyCards.Count;
        foreach(var card in historyCards)
        {
            poolManager.ReturnToPool(PoolType.CardHistory, card);
            cardsToReturn--;
        }

        yield return new WaitUntil(() => cardsToReturn <= 0);
        historyCards.Clear();
        yield return null;

    }

    public bool isSkipMode (SkipMode mode)
    {
        return mode == SkipMode.Skip;
    }
}
