using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    CardManager cardManager;
    PoolManager poolManager;
    MultiplierManager multiplierManager;
    WinLoseManager winLoseManager;
    public GamePlay gamePlay;
    public Skips Skips;
    public Deck deck;
    public RemoveCard removeCard;
    public AddCard addCard;
    public NextCard nextCard;
    public RetainCard retainCard;
    public CardHistory cardHistory;

    public void Start ()
    {
        poolManager = CommandCenter.Instance.poolManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        multiplierManager = CommandCenter.Instance.multiplierManager_;
        winLoseManager = CommandCenter.Instance.winLoseManager_;
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
            CardData cardData = cardManager.GetCardData();
            cardComponent.SetCard(
               cardData.cardSuite ,
               cardData.cardRank ,
               cardData.cardColor);

            multiplierManager.RefreshMultipliers();

            cardComponent.ShowCard();
            cardComponent.hideCardBg();
            cardComponent.HideCardOutline();
        }
    }
    public void ToggleGamePlay ()
    {
        gamePlay.ToggleGamePlay(this);
        ToggleGamePlaySkips();
    }

    public bool IsGameStarted ()
    {
        return gamePlay.isGamePlayActive;
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
        if(!Skips.AllowSkip()) yield break;
        Debug.Log($" allow skips : {Skips.AllowSkip()}");
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

            multiplierManager.RefreshMultipliers();

            Debug.Log("New card added!");
        }));
       
        Skips.SkipCard();
        yield return null;
    }

    public void ToggleGamePlaySkips ()
    {
        if (IsGameStarted())
        {
            Skips.ActivateGameplaySpins ();
        }
        else
        {
            Skips.DeactivateGameplaySpins ();
        }
    }

    public void Guess ()
    {
        if (multiplierManager.selectedMultiplier == MultiplierType.None)
        {
            Debug.LogWarning("no multiplier is selected");
            return;
        }

        StartCoroutine(guessing());
      
    }

    IEnumerator guessing ()
    {
        //next card
        yield return StartCoroutine(nextCard.nextCard(deck,cardManager));
        //enable skips
        if (Skips.IsFirstTime())
        {
            Skips.setIsFirstTime(false);
            Skips.ResetSkips();
        }

        //winsequence
        CardData prevCardData = cardManager.GetPrevCardData();
        CardData currCardData = cardManager.GetCurrentCardData();
        MultiplierType multiplierType = multiplierManager.selectedMultiplier;
        winLoseManager.outCome(prevCardData , currCardData , multiplierType);
        Debug.Log("win sequence setUp done!");
        yield return StartCoroutine(winLoseManager.WinSequence());
        //winsequence - card History

        yield return null;
    }
}
