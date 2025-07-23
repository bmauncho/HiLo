using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CardHistoryData
{
    public CardData cardData;
    public MultiplierType multiplierType;
    public OutCome outCome;
}

public class CardHistory : MonoBehaviour
{
    public int HistoryDataIndex = -1;
    public List<CardHistoryData> HistoryData = new List<CardHistoryData>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

    public void ShowHistory ()
    {
        Debug.Log(HistoryDataIndex);
        Debug.Log($"{HistoryData [HistoryDataIndex].cardData.cardSuite} \n" +
            $"{HistoryData [HistoryDataIndex].cardData.cardColor} \n" +
            $"{HistoryData [HistoryDataIndex].cardData.cardRank} \n" +
            $"{HistoryData [HistoryDataIndex].multiplierType} \n" +
            $"{HistoryData [HistoryDataIndex].outCome} \n");
    }

    public void ClearHistory ()
    {
        HistoryData.Clear();
        Debug.Log("Return history cards to pool!");
    }
}
