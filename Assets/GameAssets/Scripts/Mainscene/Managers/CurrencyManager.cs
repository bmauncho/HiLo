using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    PayOutManager payOutManager;
    public string winAmount = "2000.00";
    public string cumilativeWinAMount = "0";
    public TMP_Text walletAmountText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        payOutManager = CommandCenter.Instance.PayOutManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetTotalWinAmount ()
    {
        winAmount = payOutManager.GetWinAmount().ToString("F2");
        cumilativeWinAMount += winAmount;
        return cumilativeWinAMount;
    }
}
