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

    public void SetCard( CardSuites suite , CardRanks rank , CardColor color )
    {
        cardSuiteType = suite;
        cardRankType = rank;
        cardColor = color;

        // Set the card rank image
        cardRank.sprite = CommandCenter.Instance.cardManager_.GetCardRankSprite(rank , color);
        cardSuite_Icon.sprite = CommandCenter.Instance.cardManager_.GetCardSuiteSprite(suite , color);
        cardSuite_Bg.sprite = CommandCenter.Instance.cardManager_.GetCardSuiteSprite(suite , color);
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
        CardOutline.SetActive(true);
    }

    public void HideCardOutline ()
    {
        CardOutline.SetActive(false);
    }

}
