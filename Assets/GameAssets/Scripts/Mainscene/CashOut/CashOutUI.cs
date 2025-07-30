using TMPro;
using UnityEngine;

public class CashOutUI : MonoBehaviour
{
    public string winMultiplier = "0.00x";
    public string winAmount = "0.00";
    public TMP_Text winMultiplierText;
    public TMP_Text winAmountText;
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
    }

    public void SetWinMultiplier(string winMultiplier_ )
    {
        winMultiplier = winMultiplier_;
        UpdateWinMultiplier();
    }

    private void UpdateWinMultiplier ()
    {
        if ( winMultiplierText != null )
        {
            winMultiplierText.GetComponent<TextHelper>().ManualRefresh(winMultiplier);
        }
    }

    public void SetWinAmount(string winAmount_ )
    {
        winAmount = winAmount_;
        UpdateWinAmount();
    }

    private void UpdateWinAmount ()
    {
        if ( winAmountText != null )
        {
            winAmountText.GetComponent<TextHelper>().ManualRefresh(winAmount);
        }
    }

}
