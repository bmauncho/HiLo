using TMPro;
using UnityEngine;

public class CashOutUI : MonoBehaviour
{
    public string winMultiplier = "0.00x";
    public string winAmount = "0.00";
    public TMP_Text winMultiplierText;
    public TextHelper winMultiplierTextHelper;
    public TMP_Text winAmountText;
    public TextHelper winAmountTextHelper;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh ()
    {
        winMultiplier = "0.00x";
        winAmount = "0.00";
        SetWinMultiplier(winMultiplier);    
        SetWinAmount(winAmount);
        UpdateWinAmount();
    }

    public void SetWinMultiplier(string winMultiplier_ )
    {
        winMultiplier = winMultiplier_;
        UpdateWinMultiplier();
    }

    public void UpdateWinMultiplier ()
    {
        if ( winMultiplierText != null )
        {
            winMultiplierTextHelper.ManualRefresh(winMultiplier);
        }
    }

    public void SetWinAmount(string winAmount_ )
    {
        winAmount = winAmount_;
    }

    public void UpdateWinAmount ()
    {
        if ( winAmountText != null )
        {
            winAmountTextHelper.ManualRefresh(winAmount);
        }
    }

}
