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

        SetUp();
    }

    public void SetUp ()
    {
        if (CommandCenter.Instance)
        {
            if (CommandCenter.Instance.gameMode == GameMode.Demo)
            {
                CashAmount = 2000;
                string CASHAMOUNT = CashAmount.ToString();
                CASHAMOUNT = PrecisionFormatter.culturedFormat(CASHAMOUNT , 2);
                walletAmountText.text = CASHAMOUNT;
            }
            else
            {
                string cashamount = apiManager.GetCashAmount();
                CashAmount = double.Parse(cashamount);
                //Debug.Log($"cashamount : {cashamount}");

                walletAmountText.text = cashamount;
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

        return cumilativeWinAMount.ToString("n2" , CultureInfo.InvariantCulture); ;
    }

    public IEnumerator Bet ()
    {
        if (CommandCenter.Instance.IsDemo())
        {
            string betAmount = CommandCenter.Instance.betManager_.GetBetAmount();
            CashAmount -= double.Parse(betAmount);
        }
        else
        {
            apiManager.placeBet.Bet();
            yield return new WaitUntil(() => apiManager.placeBet.IsBetPlaced);
            CashAmount = (double)apiManager.placeBet.betResponse.new_wallet_balance;
        }
        string CASHAMOUNT = CashAmount.ToString("n2",CultureInfo.InvariantCulture);
        //CASHAMOUNT = PrecisionFormatter.culturedFormat(CASHAMOUNT , 2);
        walletAmountText.text = CASHAMOUNT;
    }


    public void CollectWinnings ()
    {
        if (CommandCenter.Instance.IsDemo())
        {
            string totalWininings = GetTotalWinAmount();
            CashAmount += double.Parse(totalWininings , CultureInfo.InvariantCulture);
        }

        if (CashAmount <= 0)
        {
            CashAmount = 0;
        }
        string CASHAMOUNT = CashAmount.ToString("n2");
        walletAmountText.text = CASHAMOUNT;
    }

    public void updateCashOutWinings ()
    {
        string totalWininings = apiManager.updateBet.updateBetResponse.new_wallet_balance;

        CashAmount = double.Parse(totalWininings,CultureInfo.InvariantCulture);

        if (CashAmount <= 0)
        {
            CashAmount = 0;
        }
        string CASHAMOUNT = CashAmount.ToString("n2");
        walletAmountText.text = CASHAMOUNT;
    }

    public bool IsMoneyDepleted ()
    {
        return CashAmount <= 0;
    }
}
