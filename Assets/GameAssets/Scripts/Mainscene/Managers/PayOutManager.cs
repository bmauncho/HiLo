using System.Collections;
using System.Globalization;
using UnityEngine;

public class PayOutManager : MonoBehaviour
{
    MultiplierManager multipliersManager;
    BetManager betManager;
    GamePlayManager gamePlayManager;
    CurrencyManager currencyManager;
    ApiManager apiManager;
    public PayOut payout;
    public CashOutUI CashOutUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multipliersManager = CommandCenter.Instance.multiplierManager_;
        betManager = CommandCenter.Instance.betManager_;
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
        currencyManager = CommandCenter.Instance.currencyMan_;
        apiManager = CommandCenter.Instance.apiManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void ShowPayOut ()
    {
       StartCoroutine(payout.ShowPayOut (this));
    }

    public IEnumerator updatePayout ()
    {
        string winAmount = "";
        string winMultiplier = "";
        if (CommandCenter.Instance.IsDemo())
        {
            winAmount = currencyManager.GetTotalWinAmount();
            winMultiplier = GetWinMultiplier();
        }
        else
        {
            bool isDone = apiManager.updateBet.isUpdated;
            apiManager.updateBet.UpdateTheBet();
            yield return new WaitUntil(() => isDone);
            winAmount = apiManager.updateBet.new_wallet_balance.ToString("N2" , CultureInfo.CurrentCulture);
        }
           
        CashOutUI.SetWinAmount(winAmount);
        CashOutUI.SetWinMultiplier(winMultiplier+"x");
        gamePlayManager.gamePlay.SetCashOutAmount(winAmount);
        CommandCenter.Instance.currencyMan_.CollectWinnings();
    }

    public void resetPayout ()
    {
        string winAmount = "0.00";
        string winMultiplier = "0.00x";
        CashOutUI.SetWinAmount(winAmount);
        CashOutUI.SetWinMultiplier(winMultiplier);
    }

    public float GetWinAmount ()
    {
        int cardValue = (int)multipliersManager.GetSelectedMultiplierType();
        string betAmount = betManager.GetBetAmount();
        float parsedbetAmount;
        if (!float.TryParse(betAmount , out parsedbetAmount))
        {
            Debug.LogWarning($"Invalid multiplier format: '{betAmount}'");
            parsedbetAmount = 0f; // or any default value you consider safe
        }
        float winAmount = cardValue * parsedbetAmount * multiplier();

        return winAmount;
    }

    public string GetWinMultiplier ()
    {
        return multipliersManager.GetMultiplier();
    }

    public float multiplier ()
    {
        string multiplier = GetWinMultiplier().ToString();

        multiplier = multiplier.TrimEnd();

        float parsedMultiplier;
        if (!float.TryParse(multiplier , out parsedMultiplier))
        {
            Debug.LogWarning($"Invalid multiplier format: '{multiplier}'");
            parsedMultiplier = 0f; // or any default value you consider safe
        }
        return parsedMultiplier;
    }
}
