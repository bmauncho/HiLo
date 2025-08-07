using System;
using System.Collections;
using UnityEngine;

public class NextCard : MonoBehaviour
{
    GamePlayManager gamePlayManager;
    CurrencyManager currencyManager;
    ApiManager apiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
        currencyManager = CommandCenter.Instance.currencyMan_;
        apiManager = CommandCenter.Instance .apiManager_;
    }

    public IEnumerator nextCard ( Deck deck,CardManager cardManager)
    {
        Card cardComponenet = deck.newCard.GetTheOwner().GetComponent<Card>();
        if (!CommandCenter.Instance.IsDemo())
        {
            yield return StartCoroutine(currencyManager.Bet());
            yield return StartCoroutine(guess());
        }
        CardData cardData = cardManager.GetCardData();

        yield return null;
    }
    IEnumerator guess ()
    {
        if (!CommandCenter.Instance.IsDemo())
        {
            apiManager.guessApi.Guess();
            yield return new WaitUntil(() => apiManager.guessApi.IsGuessDone);
        }
    }
}
