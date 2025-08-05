using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    MultiplierManager multipliersManager;
    WinLoseManager winLoseManager;
    CardManager cardManager;
    BetManager betManager;
    PayOutManager payOutManager;
    ApiManager apiManager;
    public bool isGamePlayActive = false;
    public GameObject start;
    public GameObject cashOut;
    public TMP_Text cashOutAmount;
    bool isGameOver = false;
    public Button cashOutButton;

    private void Start ()
    {
        multipliersManager = CommandCenter.Instance.multiplierManager_;
        winLoseManager = CommandCenter.Instance.winLoseManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        betManager = CommandCenter.Instance.betManager_;
        payOutManager = CommandCenter.Instance.PayOutManager_;
        apiManager = CommandCenter.Instance.apiManager_;
    }

    public void showStart ()
    {
        start.SetActive(true);
    }

    public void hideStart ()
    {
        start.SetActive(false);
    }

    public void showCashOut (bool isFirstTime =false)
    {
        if (isFirstTime)
        {
            cashOutButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            cashOutButton.GetComponent<Button>().interactable = true;
        }

        cashOut.SetActive(true);
    }

    public void SetCashOutButtonInteractivity(bool interactable )
    {
        cashOutButton.GetComponent<Button>().interactable = interactable;
    }

    public void hideCashOut ()
    {
        cashOut.SetActive(false);
    }

    public IEnumerator ToggleGamePlay (GamePlayManager gamePlayManager)
    {
        if (!isGamePlayActive)
        {
            hideStart();
            showCashOut(true);
            isGamePlayActive = true;
            betManager.Bet.IncreaseBtn.ActivateMask();
            betManager.Bet.DecreaseBtn.ActivateMask();
            yield return StartCoroutine(startSession());
            gamePlayManager.SetIsFirstTime(true);
            ResetCashAmount();
            SetCashOutAmount("0.00");
            multipliersManager.disableGuessMask();
            multipliersManager.enableGuessBtns();
            yield return StartCoroutine(gamePlayManager.cardHistory.ResetHistoryData());
            gamePlayManager.GetActiveCard().GetComponent<Card>().resetCardforGamePlay();
            gamePlayManager.cardHistory.ShowHistory();
            multipliersManager.Multipliers.ToggleMultiplier(cardManager.GetCurrentCardData());
            multipliersManager.Multipliers.ToggleMultiplierType(cardManager.GetCurrentCardData());
            multipliersManager.RefreshMultipliers();
            multipliersManager.Multipliers.UpdateText();
            payOutManager.resetPayout();
        }
        else
        {
            isGameOver = false;
            payOutManager.payout.PayoutEffectComplete +=OnEffectComplete;
            showStart();
            hideCashOut();
            yield return StartCoroutine(endSession());
            isGamePlayActive = false;
            multipliersManager.enableGuessMask();
            multipliersManager.disableGuessBtns();
            gamePlayManager.Skips.setIsFirstTime(true);
            gamePlayManager.SetIsFirstTime(false);
            payOutManager.ShowPayOut();
            yield return new WaitUntil(() => isGameOver);
            //yield return StartCoroutine(gamePlayManager.cardHistory.ResetHistoryData());
            betManager.Bet.IncreaseBtn.DeactivateMask();
            betManager.Bet.DecreaseBtn.DeactivateMask();
            payOutManager.payout.PayoutEffectComplete -= OnEffectComplete;
            gamePlayManager.resetPlayCounter();
            apiManager.SkipApi.ResetSkipInit();

            winLoseManager.resetOutCome();
        }

        gamePlayManager.ToggleGamePlaySkips();
    }

    public void SetCashOutAmount ( string amount )
    {
        cashOutAmount.text = amount;
        GetComponentInChildren<TextHelper>().ManualRefresh(amount);
    }

    public void ResetCashAmount ()
    {
        SetCashOutAmount("0.00");
    }

    public void OnEffectComplete ()
    {
        isGameOver = true;
    }

    IEnumerator startSession ()
    {
        if (!CommandCenter.Instance.IsDemo())
        {
            apiManager.StartApi.startGame();

            yield return new WaitUntil(() => apiManager.StartApi.IsStartDone);
            Debug.Log("session started!");
        }

    }

    IEnumerator endSession ()
    {
        if(!CommandCenter.Instance.IsDemo() && winLoseManager.GetTheOutCome() != OutCome.Lose)
        {
            apiManager.cashOutApi.CashOut();
            yield return new WaitUntil(() => apiManager.cashOutApi.IsCashOutDone);
            double winAMount = apiManager.cashOutApi.cashOutResponse.game_state.final_win;
            apiManager.updateBet.SetAmountWon(winAMount);
            apiManager.updateBet.UpdateTheBet();
            yield return new WaitUntil(() => apiManager.updateBet.isUpdated);
            string winAmount = apiManager.updateBet.new_wallet_balance.ToString("N2" , CultureInfo.CurrentCulture);
            Debug.Log("session ended!");

        }
    }

}

