using System.Collections;
using TMPro;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    MultiplierManager multipliersManager;
    WinLoseManager winLoseManager;
    CardManager cardManager;
    BetManager betManager;
    PayOutManager payOutManager;
    public bool isGamePlayActive = false;
    public GameObject start;
    public GameObject cashOut;
    public TMP_Text cashOutAmount;
    bool isGameOver = false;

    private void Start ()
    {
        multipliersManager = CommandCenter.Instance.multiplierManager_;
        winLoseManager = CommandCenter.Instance.winLoseManager_;
        cardManager = CommandCenter.Instance.cardManager_;
        betManager = CommandCenter.Instance.betManager_;
        payOutManager = CommandCenter.Instance.PayOutManager_;
    }

    public void showStart ()
    {
        start.SetActive(true);
    }

    public void hideStart ()
    {
        start.SetActive(false);
    }

    public void showCashOut ()
    {
        cashOut.SetActive(true);
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
            showCashOut();
            isGamePlayActive = true;
            SetCashOutAmount("0.00");
            multipliersManager.disableGuessMask();
            multipliersManager.enableGuessBtns();
            multipliersManager.RefreshMultipliers();
            yield return StartCoroutine(gamePlayManager.cardHistory.ResetHistoryData());
            gamePlayManager.GetActiveCard().GetComponent<Card>().resetCardforGamePlay();
            gamePlayManager.cardHistory.ShowHistory();
            betManager.Bet.IncreaseBtn.ActivateMask();
            betManager.Bet.DecreaseBtn.ActivateMask();
            multipliersManager.Multipliers.ToggleMultiplier(cardManager.GetCurrentCardData());
        }
        else
        {
            isGameOver = false;
            payOutManager.payout.PayoutEffectComplete +=OnEffectComplete;
            showStart();
            hideCashOut();
            isGamePlayActive = false;
            multipliersManager.enableGuessMask();
            multipliersManager.disableGuessBtns();
            gamePlayManager.Skips.setIsFirstTime(true);
            payOutManager.ShowPayOut();
            yield return new WaitUntil(() => isGameOver);
            winLoseManager.resetOutCome();
            yield return StartCoroutine(gamePlayManager.cardHistory.ResetHistoryData());
            betManager.Bet.IncreaseBtn.DeactivateMask();
            betManager.Bet.DecreaseBtn.DeactivateMask();
            payOutManager.payout.PayoutEffectComplete -= OnEffectComplete;
        }
    }

    public void SetCashOutAmount ( string amount )
    {
        cashOutAmount.text = amount;
        GetComponentInChildren<TextHelper>().ManualRefresh(amount);
    }

    public void OnEffectComplete ()
    {
        isGameOver = true;
    }

}

