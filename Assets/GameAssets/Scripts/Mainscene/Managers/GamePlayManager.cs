using DG.Tweening;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    CardManager cardManager;
    PoolManager poolManager;
    MultiplierManager multiplierManager;
    WinLoseManager winLoseManager;
    BetManager betManager;
    PayOutManager payOutManager;
    public GamePlay gamePlay;
    public Skips Skips;
    public Deck deck;
    public RemoveCard removeCard;
    public AddCard addCard;
    public NextCard nextCard;
    public RetainCard retainCard;
    public CardHistory cardHistory;
    public GameObject ActiveCard;
    bool IsSkips = false;

    public void Start ()
    {
        poolManager = CommandCenter.Instance.poolManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        multiplierManager = CommandCenter.Instance.multiplierManager_;
        winLoseManager = CommandCenter.Instance.winLoseManager_;
        betManager = CommandCenter.Instance.betManager_;
        payOutManager = CommandCenter.Instance.PayOutManager_ ;
        Invoke(nameof(SetActiveCard),.25f);
    }

    public void SetActiveCard ()
    {
        Transform transform = deck.newCard.transform;
        GameObject card = poolManager.GetFromPool(PoolType.Cards , transform.position , Quaternion.identity,transform);
        card.transform.localPosition = Vector3.zero;
        deck.newCard.SetOwner(card);
        Card cardComponent = card.GetComponent<Card>();
        SetActiveCard(card);
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
        multiplierManager.Multipliers.ToggleMultiplier(cardManager.GetCurrentCardData());
        multiplierManager.disableGuessBtns();
    }

    public void ToggleGamePlay ()
    {
        StartCoroutine( gamePlay.ToggleGamePlay(this));
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
        IsSkips =false;
        if (!IsGameStarted())
        {
            if(winLoseManager.GetTheOutCome() == OutCome.None ||
                winLoseManager.GetTheOutCome() == OutCome.Lose)
            {
                GetActiveCard().GetComponent<Card>().resetCardforGamePlay();
                yield return StartCoroutine(cardHistory.ResetHistoryData());
            }
        }

        StartCoroutine(removeCard.removeCurrentCard(deck,poolManager));
        
        yield return new WaitForSeconds(0.1f);
        addCard.OnComplete += canSkip;
        yield return StartCoroutine(addCard.addNewCard(deck , poolManager , () =>
        {
            GameObject card = deck.newCard.GetTheOwner();
            SetActiveCard(card);
            Card cardComponenet = card.GetComponent<Card>();
            CardData cardData = cardManager.GetCardData();
            cardComponenet.SetCard(
                cardData.cardSuite,
                cardData.cardRank,
                cardData.cardColor);

            if(IsGameStarted())
            {
                multiplierManager.refreshMultplierValues(multiplierManager.GetCurrentMultipliers(true));
            }
            else
            {

                multiplierManager.RefreshMultipliers();
            }


            Debug.Log("New card added!");
        }));
        yield return new WaitUntil(() => IsSkips);
        addCard.OnComplete -= canSkip;
        Skips.SkipCard();
        yield return null;
    }

    void canSkip ()
    {
        IsSkips = true;
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
        Debug.Log($"Selected multiplier : {multiplierType}");
        MultiplierType newMultiplierType = whichSelectedMultiplier(prevCardData, currCardData,multiplierType);
        Debug.Log($"multipler type : {newMultiplierType.ToString()}");
        winLoseManager.outCome(prevCardData , currCardData , newMultiplierType);
        Debug.Log("win sequence setUp done!");
        yield return StartCoroutine(winLoseManager.WinSequence());
        //winsequence - card History
        cardHistory.ShowHistory();
        multiplierManager.Multipliers.ToggleMultiplier(cardManager.GetCurrentCardData());
        yield return new WaitForSeconds(.1f);
        onWin();
        onlose();
        yield return null;
    }

    public MultiplierType whichSelectedMultiplier (
    CardData prevCard ,
    CardData currCard ,
    MultiplierType selectedMultiplier )
    {
        int prev = (int)prevCard.cardRank;
        int curr = (int)currCard.cardRank;
        int totalRanks = Enum.GetValues(typeof(CardRanks)).Length;
        int ace = 0;
        int king = totalRanks - 1;

        switch (selectedMultiplier)
        {
            case MultiplierType.High:
                if (prev == ace && curr > prev)
                {
                    return MultiplierType.High;
                }
                else 
                {
                    return MultiplierType.HighOrSame;
                }

            case MultiplierType.Low:
                if (prev == king && curr < prev)
                {
                    return MultiplierType.Low;
                }
                else 
                {
                    return MultiplierType.LowOrSame;
                }

            case MultiplierType.Same:
                return MultiplierType.Same;
        }

        return MultiplierType.None;
    }

    void onlose ()
    {
        if(winLoseManager.GetTheOutCome() == OutCome.None ||
            winLoseManager.GetTheOutCome()== OutCome.Win)
        {
            return;
        }

        gamePlay.isGamePlayActive = false;
        multiplierManager.enableGuessMask();
        multiplierManager.disableGuessBtns();
        Skips.setIsFirstTime(true);
        Skips.ResetSkips();
        winLoseManager.resetOutCome();
        betManager.Bet.IncreaseBtn.DeactivateMask();
        betManager.Bet.DecreaseBtn.DeactivateMask();
        payOutManager.resetPayout();
    }

    void onWin ()
    {
        if (winLoseManager.GetTheOutCome() == OutCome.None ||
           winLoseManager.GetTheOutCome() == OutCome.Lose)
        {
            return;
        }

        payOutManager.updatePayout();
    }

    public void SetActiveCard(GameObject card )
    {
        ActiveCard = card;
    }

    public GameObject GetActiveCard() { return ActiveCard; }

}
