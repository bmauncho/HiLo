using System.Collections;
using UnityEngine;
public enum OutCome
{
    None,
    Win,
    Lose,
}
public class WinLoseManager : MonoBehaviour
{
    GamePlayManager gamePlayManager;
    PoolManager poolManager;
    CardManager cardManager;
    MultiplierManager multiplierManager;
    BetManager betManager;
    public OutCome TheOutCome;
    private bool IsAddNewCardComplete = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
        poolManager = CommandCenter.Instance.poolManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        multiplierManager = CommandCenter.Instance.multiplierManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetOutCome(OutCome outCome )
    {
        TheOutCome = outCome;
    }

    public void outCome (
        CardData prevCard , 
        CardData currCard,
        MultiplierType selectedMultiplier )
    {
        bool isWin = IsWin(
            prevCard.cardRank ,
            currCard.cardRank ,
            selectedMultiplier);

        Debug.Log($"Testing: {prevCard.cardRank} vs {currCard.cardRank}, Guess: {selectedMultiplier}, Result: {isWin}");

        if (isWin)
        {
            SetOutCome(OutCome.Win);
        }
        else
        {
            SetOutCome (OutCome.Lose);
        }
    }

    public OutCome GetTheOutCome ()
    {
        return TheOutCome;
    }

    private bool IsWin ( CardRanks currentCard , CardRanks nextCard , MultiplierType selectedMultiplier )
    {
        return selectedMultiplier switch
        {
            MultiplierType.higher => nextCard > currentCard,
            MultiplierType.higher_or_same => nextCard >= currentCard,
            MultiplierType.lower => nextCard < currentCard,
            MultiplierType.lower_or_same => nextCard <= currentCard,
            MultiplierType.same => nextCard == currentCard,
            _ => false
        };
    }


    public IEnumerator WinSequence ()
    {
        if(TheOutCome == OutCome.Win)
        {
            yield return StartCoroutine(win());

        }
        else
        {
            yield return StartCoroutine(lose());
        }

        yield return null;
    }

    IEnumerator win ()
    {
        IsAddNewCardComplete = false;
        Debug.Log("win");
        //update card history
        Debug.Log("Update card History!");

        //show next card
        Deck deck = gamePlayManager.deck;
        AddCard addCard = gamePlayManager.addCard;
        RemoveCard removeCard = gamePlayManager.removeCard;

        addCard.OnComplete += OnAddCardComplete;

        StartCoroutine(removeCard.removeCurrentCard(deck , poolManager));
        CommandCenter.Instance.soundManager_.PlaySound("CorrectGuess");
        yield return new WaitForSeconds(.1f);
        yield return StartCoroutine(addCard.addNewCard(deck , poolManager , () =>
        {
            GameObject card = deck.newCard.GetTheOwner();
            gamePlayManager.SetActiveCard(card);
            Card cardComponenet = card.GetComponent<Card>();
            CardData cardData = cardManager.GetCurrentCardData();
            cardComponenet.SetCard(
                cardData.cardSuite ,
                cardData.cardRank ,
                cardData.cardColor);

            if (gamePlayManager.IsGameStarted())
            {
                multiplierManager.refreshMultplierValues(multiplierManager.GetCurrentMultipliers());
            }
            else
            {
                multiplierManager.RefreshMultipliers();
            }


            Debug.Log("New card added-win!");
        }));
        yield return new WaitUntil(()=>IsAddNewCardComplete);
        //update multilier 

        Debug.Log("Update Multiplier");

        addCard.OnComplete -= OnAddCardComplete;

        yield return null;
    }

    IEnumerator lose ()
    {
        IsAddNewCardComplete = false;
        Debug.Log("lose");
        //update card history
        //show next card
        Deck deck = gamePlayManager.deck;
        AddCard addCard = gamePlayManager.addCard;
        RemoveCard removeCard = gamePlayManager.removeCard;

        addCard.OnComplete += OnAddCardComplete;

        StartCoroutine(removeCard.removeCurrentCard(deck , poolManager));
        yield return new WaitForSeconds(.1f);
        yield return StartCoroutine(addCard.addNewCard(deck , poolManager , () =>
        {
            GameObject card = deck.newCard.GetTheOwner();
            Card cardComponenet = card.GetComponent<Card>();
            CardData cardData = cardManager.GetCurrentCardData();
            gamePlayManager.SetActiveCard(card);
            cardComponenet.SetCard(
                cardData.cardSuite ,
                cardData.cardRank ,
                cardData.cardColor);

            multiplierManager.refreshMultplierValues(multiplierManager.GetCurrentMultipliers());

            Debug.Log("New card added-win!");
        }));
        yield return new WaitUntil(() => IsAddNewCardComplete);
        yield return new WaitForSeconds(.25f);
        //lose anim on prev card

        RetainCard retainCard = gamePlayManager.retainCard;
        gamePlayManager.Skips.DeactivateGameplaySpins();
        gamePlayManager.gamePlay.showStart();
        gamePlayManager.gamePlay.hideCashOut();
        yield return StartCoroutine(retainCard.loseAnim(deck));
        addCard.OnComplete -= OnAddCardComplete;
        //update multilier 
        //prev card is set as the current card
        //cardManager.ResetCardData();

        yield return null;
    }

    public void resetOutCome ()
    {
        TheOutCome = OutCome.None;
    }
    private void OnAddCardComplete ()
    {
        IsAddNewCardComplete = true;
    }
}
