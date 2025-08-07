using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardSuites cardSuiteType;
    public CardRanks cardRankType;
    public CardColor cardColor;

    [Header("Images")]
    public Image cardRank;
    public Image cardSuite_Icon;
    public Image cardSuite_Bg;

    public GameObject CardData;
    public GameObject CardBg;
    public GameObject CardOutline;
    public GameObject CardMask;

    public void SetCard( CardSuites suite , CardRanks rank , CardColor color )
    {
        cardSuiteType = suite;
        cardRankType = rank;
        cardColor = color;

        // Set the card rank image
        cardRank.sprite = CommandCenter.Instance.cardManager_.GetCardRankSprite(rank , color);
        cardSuite_Icon.sprite = CommandCenter.Instance.cardManager_.GetCardSuiteSprite(suite , color);
        cardSuite_Bg.sprite = 
            IsFaceCard() ? 
            CommandCenter.Instance.cardManager_.GetCardFaceRankSprite(rank , color): 
            CommandCenter.Instance.cardManager_.GetCardSuiteSprite(suite , color);
    }

    public void ShowCard ()
    {
        CardData.SetActive(true);
    }

    public void HideCard ()
    {
        CardData.SetActive(false);
    }

    public void showCardBg ()
    {
        CardBg.SetActive(true);
    }

    public void hideCardBg ()
    {
        CardBg.SetActive(false);
    }

    public void ShowCardOutline ()
    {
        Debug.Log("Show card Outine!");
        CardOutline.SetActive(true);
    }

    public void HideCardOutline ()
    {
        CardOutline.SetActive(false);
    }

    public void ShowMask ()
    {
        CardMask.SetActive(true);
    }

    public void HideMask ()
    {
        CardMask.SetActive(false);
    }

    public bool IsFaceCard ()
    {
        return cardRankType == CardRanks.JACK || cardRankType == CardRanks.QUEEN || cardRankType == CardRanks.KING;
    }

    public void resetCardforGamePlay ()
    {
        HideCardOutline();
        //Debug.Log("Hide card Outine!");
    }
}
