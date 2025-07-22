using TMPro;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    MultiplierManager multipliersManager;
    WinLoseManager winLoseManager;
    public bool isGamePlayActive = false;
    public GameObject start;
    public GameObject cashOut;
    public TMP_Text cashOutAmount;

    private void Start ()
    {
        multipliersManager = CommandCenter.Instance.multiplierManager_;
        winLoseManager = CommandCenter.Instance.winLoseManager_;
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

    public void ToggleGamePlay (GamePlayManager gamePlayManager)
    {
        if (!isGamePlayActive)
        {
            hideStart();
            showCashOut();
            isGamePlayActive = true;
            SetCashOutAmount("0.00");
            multipliersManager.disableGuessMask();
            multipliersManager.enableGuessBtns();
        }
        else
        {
            showStart();
            hideCashOut();
            isGamePlayActive = false;
            multipliersManager.enableGuessMask();
            multipliersManager.disableGuessBtns();
            gamePlayManager.Skips.setIsFirstTime(true);
            winLoseManager.resetOutCome();
        }
    }

    public void SetCashOutAmount ( string amount )
    {
        cashOutAmount.text = amount;
        GetComponentInChildren<TextHelper>().ManualRefresh(amount);
    }

}

