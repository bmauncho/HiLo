using TMPro;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public bool isGamePlayActive = false;
    public GameObject start;
    public GameObject cashOut;
    public TMP_Text cashOutAmount;


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

    public void ToggleGamePlay ()
    {
        if (!isGamePlayActive)
        {
            hideStart();
            showCashOut();
            isGamePlayActive = true;
            SetCashOutAmount("0.00");
        }
        else
        {
            showStart();
            hideCashOut();
            isGamePlayActive = false;

        }
    }

    public void SetCashOutAmount ( string amount )
    {
        cashOutAmount.text = amount;
        GetComponentInChildren<TextHelper>().ManualRefresh(amount);
    }

}

