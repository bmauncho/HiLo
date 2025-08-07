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
    CurrencyManager currencyManager;
    ApiManager apiManager;
    public GameObject ActiveCard;
    public GamePlay gamePlay;
    public Skips Skips;
    public Deck deck;
    public RemoveCard removeCard;
    public AddCard addCard;
    public NextCard nextCard;
    public RetainCard retainCard;
    public CardHistory cardHistory;
    bool IsSkips = false;
    public CashOutUI cashOutUI;
    public bool IsFirstTime = false;
    public bool IsSkip = false;
    bool isGuessing = false;
    bool isFromSkipping = false;
    bool isFromGamePlay = false;
    [SerializeField]bool isSkipping = false;
    public void Start ()
    {
        poolManager = CommandCenter.Instance.poolManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        multiplierManager = CommandCenter.Instance.multiplierManager_;
        winLoseManager = CommandCenter.Instance.winLoseManager_;
        betManager = CommandCenter.Instance.betManager_;
        payOutManager = CommandCenter.Instance.PayOutManager_ ;
        currencyManager = CommandCenter.Instance.currencyMan_ ;
        apiManager = CommandCenter.Instance.apiManager_;
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
        CardData cardData = new CardData();
        if (cardComponent != null)
        {
            cardData = cardManager.GetCardData();
            cardComponent.SetCard(
               cardData.cardSuite ,
               cardData.cardRank ,
               cardData.cardColor);

            

            cardComponent.ShowCard();
            cardComponent.hideCardBg();
            cardComponent.HideCardOutline();
        }
        multiplierManager.Multipliers.ToggleMultiplier(cardData);
        multiplierManager.Multipliers.ToggleMultiplierType(cardData);
        multiplierManager.RefreshMultipliers();
        multiplierManager.Multipliers.UpdateText();
        multiplierManager.disableGuessBtns();
    }

    public void ToggleGamePlay ()
    {
        StartCoroutine(gamePlay.ToggleGamePlay(this));
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
        if (isSkipping)
        {
            return;
        }
        isSkipping = true;
        StartCoroutine(SkipCoroutine());
    }

    IEnumerator SkipCoroutine ()
    {
        if (!Skips.AllowSkip())
        {
            isSkipping = false;
            yield break;
        }
        CommandCenter.Instance.soundManager_.PlaySound("SkipButton");
        Debug.Log($" allow skips : {Skips.AllowSkip()}");
        IsSkips = false;
        SetIsSkip(true);
        if (!IsGameStarted())
        {
            if(winLoseManager.GetTheOutCome() == OutCome.None ||
                winLoseManager.GetTheOutCome() == OutCome.Lose)
            {
                GetActiveCard().GetComponent<Card>().resetCardforGamePlay();
                yield return StartCoroutine(cardHistory.ResetHistoryData());
                cashOutUI.Refresh();
            }
        }

        if(!CommandCenter.Instance.IsDemo())
        {
            if (IsGameStarted())
            {
                apiManager.SkipApi.Skip();
                yield return new WaitUntil(() => apiManager.SkipApi.IsSkiped);
            }
            else
            {
                apiManager.previewSkipApi.previewSkip();
                yield return new WaitUntil(() => apiManager.previewSkipApi.IsPreviewSkipDone);
            } 
        }

        StartCoroutine(removeCard.removeCurrentCard(deck , poolManager));
        
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

            multiplierManager.Multipliers.ToggleMultiplier(cardData);
            multiplierManager.Multipliers.ToggleMultiplierType(cardData);
            multiplierManager.Multipliers.UpdateText();

            if(IsGameStarted())
            {
                multiplierManager.refreshMultplierValues(multiplierManager.GetCurrentMultipliers(true));
            }
            else
            {
                multiplierManager.RefreshMultipliers();
            }


           // Debug.Log("New card added!");
        }));
        yield return new WaitUntil(() => IsSkips);
        addCard.OnComplete -= canSkip;
        Skips.SkipCard();
        setisfromskiping(true);
        isSkipping = false;
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
        if (isGuessing)
        {
            return;
        }

        isGuessing = true;
        gamePlay.SetCashOutButtonInteractivity (false);
        if (multiplierManager.selectedMultiplier == MultiplierType.None)
        {
            Debug.LogWarning("no multiplier is selected");
            return;
        }
        cashOutUI.Refresh();
        SetIsSkip(false);
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
            gamePlay.showCashOut(false);
            apiManager.SetIsFirstPlayDone(true);
        }

        //bet
        SetIsFirstTime(false);

        //winsequence
        CardData prevCardData = cardManager.GetPrevCardData();
        CardData currCardData = cardManager.GetCurrentCardData();
        MultiplierType multiplierType = multiplierManager.selectedMultiplier;
        winLoseManager.outCome(prevCardData , currCardData , multiplierType);
        Skips.refreshSkips();
        //Debug.Log("win sequence setUp done!");
        yield return StartCoroutine(winLoseManager.WinSequence());
        yield return new WaitForSeconds(.1f);

        if(winLoseManager.GetTheOutCome() == OutCome.Win)
        {
            yield return StartCoroutine(onWin());
        }
        else
        {
            yield return StartCoroutine(onlose());
            SetIsFromGamePlay(true);

        } 
        isGuessing = false;
        setisfromskiping(false);
        gamePlay.SetCashOutButtonInteractivity(true);
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
            case MultiplierType.higher:
                if (prev == ace && curr > prev)
                {
                    return MultiplierType.higher;
                }
                else 
                {
                    return MultiplierType.higher_or_same;
                }

            case MultiplierType.lower:
                if (prev == king && curr < prev)
                {
                    return MultiplierType.lower;
                }
                else 
                {
                    return MultiplierType.lower_or_same;
                }

            case MultiplierType.same:
                return MultiplierType.same;
        }

        return MultiplierType.None;
    }

    IEnumerator onlose ()
    {
        if(winLoseManager.GetTheOutCome() == OutCome.None ||
            winLoseManager.GetTheOutCome()== OutCome.Win)
        {
            yield break;
        }

        gamePlay.isGamePlayActive = false;
        multiplierManager.enableGuessMask();
        multiplierManager.disableGuessBtns();
        Skips.setIsFirstTime(true);
        Skips.ResetSkips();
        betManager.Bet.IncreaseBtn.DeactivateMask();
        betManager.Bet.DecreaseBtn.DeactivateMask();
        payOutManager.resetPayout();
    }

    IEnumerator onWin ()
    {
        if (winLoseManager.GetTheOutCome() == OutCome.None ||
           winLoseManager.GetTheOutCome() == OutCome.Lose)
        {
           yield break;
        }

        yield return StartCoroutine(payOutManager.updatePayout());
    }

    public void SetActiveCard(GameObject card )
    {
        ActiveCard = card;
    }

    public GameObject GetActiveCard() { return ActiveCard; }

    public void SetIsFirstTime(bool value )
    {
        IsFirstTime = value;
    }

    public void SetIsSkip(bool value )
    {
        IsSkip = value;
    }

    public void setisfromskiping(bool value )
    {
        isFromSkipping = value;
    }

    public void SetIsFromGamePlay (bool value)
    {
        isFromGamePlay = value;
    }

    public bool Get_IsSkip() { return IsSkip; }
    public bool Get_IsFirstTime() { return IsFirstTime; }

    public bool GetIsFromSkipping () { return isFromSkipping; }

    public bool GetIsFromGameplay () { return isFromGamePlay; }

    public void resetPlayCounter ()
    {
        apiManager.SetIsFirstPlayDone(false);
        Skips.setIsFirstTime(true);
        Skips.ResetSkips();
    }

}
