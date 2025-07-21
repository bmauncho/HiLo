using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    CardManager cardManager;
    PoolManager poolManager;
    public GamePlay gamePlay;
    public Skips Skips;
    public Deck deck;
    public RemoveCard removeCard;
    public AddCard addCard;

    public void Start ()
    {
        poolManager = CommandCenter.Instance.poolManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        Invoke(nameof(SetActiveCard),.25f);
    }

    public void SetActiveCard ()
    {
        Transform transform = deck.newCard.transform;
        GameObject card = poolManager.GetFromPool(PoolType.Cards , transform.position , Quaternion.identity,transform);
        card.transform.localPosition = Vector3.zero;
        deck.newCard.SetOwner(card);
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent != null)
        {
            cardComponent.ShowCard();
            cardComponent.hideCardBg();
            cardComponent.HideCardOutline();
        }
    }
    public void ToggleGamePlay ()
    {
        gamePlay.ToggleGamePlay();
    }

    public void SetCashOutAmount ( string amount )
    {
        gamePlay.SetCashOutAmount(amount);
    }

    public void Skip ()
    {
        StartCoroutine(SkipCoroutine());
    }

    IEnumerator SkipCoroutine ()
    {
        StartCoroutine(removeCard.removeCurrentCard(deck,poolManager));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(addCard.addNewCard(deck , poolManager , () =>
        {
            Card cardComponenet = deck.newCard.GetTheOwner().GetComponent<Card>();
            CardData cardData = cardManager.GetCardData();
            cardComponenet.SetCard(
                cardData.cardSuite,
                cardData.cardRank,
                cardData.cardColor);

            Debug.Log("New card added!");
        }));
        Skips.SkipCard();
        yield return null;
    }
}
