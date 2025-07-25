using UnityEngine;

public class PayOutManager : MonoBehaviour
{
    MultiplierManager multipliersManager;
    BetManager betManager;
    GamePlayManager gamePlayManager;
    public PayOut payout;
    public CashOutUI CashOutUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multipliersManager = CommandCenter.Instance.multiplierManager_;
        betManager = CommandCenter.Instance.betManager_;
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void ShowPayOut ()
    {
       StartCoroutine(payout.ShowPayOut (this));
    }

    public void updatePayout ()
    {
        string winAmount = GetWinAmount().ToString("F2");
        string winMultiplier = GetWinMultiplier();
        CashOutUI.SetWinAmount(winAmount);
        CashOutUI.SetWinMultiplier(winMultiplier);
        gamePlayManager.gamePlay.SetCashOutAmount(winAmount);
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
