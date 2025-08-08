using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    PayOutManager payOutManager;
    ApiManager apiManager;
    GamePlayManager gamePlayManager;
    public double CashAmount;
    public double winAmount;
    public double cumilativeWinAMount;
    public TMP_Text walletAmountText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        payOutManager = CommandCenter.Instance.PayOutManager_;
        apiManager = CommandCenter.Instance.apiManager_;
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;

        if (CommandCenter.Instance)
        {
            if(CommandCenter.Instance.gameMode == GameMode.Demo)
            {
                CashAmount = 2000;
                walletAmountText.text = CashAmount.ToString("N2" , CultureInfo.CurrentCulture); ;
            }
            else
            {
                string cashamount = apiManager.GetCashAmount();
                if(double.TryParse(cashamount,out double amount))
                {
                    CashAmount += amount;
                }
                
                walletAmountText.text = CashAmount.ToString("N2" , CultureInfo.CurrentCulture); ;
            }   
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetTotalWinAmount ()
    {
        winAmount = payOutManager.GetWinAmount();
        if (CommandCenter.Instance.IsDemo())
        {
            cumilativeWinAMount += winAmount;
        }
        else
        {
            cumilativeWinAMount = winAmount;
        }

        return cumilativeWinAMount.ToString("N2" , CultureInfo.CurrentCulture); ;
    }

    public IEnumerator Bet ()
    {
        if (CommandCenter.Instance.IsDemo())
        {
            string betAmount = CommandCenter.Instance.betManager_.GetBetAmount();

            if (double.TryParse(betAmount , out double bet))
            {
                CashAmount -= bet;
            }
            else
            {
                Debug.LogWarning($"Invalid bet amount: {betAmount}");
            }
        }
        else
        {
            apiManager.placeBet.Bet();
            yield return new WaitUntil(() => apiManager.placeBet.IsBetPlaced);
            CashAmount = (double)apiManager.placeBet.betResponse.new_wallet_balance;
        }
        walletAmountText.text = CashAmount.ToString("N2" , CultureInfo.CurrentCulture); ;
    }


    public void CollectWinnings ()
    {
        if (CommandCenter.Instance.IsDemo())
        {
            string totalWininings = GetTotalWinAmount();
            if (double.TryParse(totalWininings , out double winnings))
            {
                CashAmount += winnings;
            }
        }

        walletAmountText.text = CashAmount.ToString("N2" , CultureInfo.CurrentCulture);
    }

    public void updateCashOutWinings ()
    {
        string totalWininings = apiManager.updateBet.updateBetResponse.new_wallet_balance;
        if (double.TryParse(totalWininings , out double winnings))
        {
            CashAmount = winnings;
        }
        walletAmountText.text = CashAmount.ToString("N2" , CultureInfo.CurrentCulture);
    }

    public bool IsMoneyDepleted ()
    {
        return CashAmount < 0;
    }
}
