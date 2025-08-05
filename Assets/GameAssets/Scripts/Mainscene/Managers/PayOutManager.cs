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

    public  void ShowPayOut ()
    {
       StartCoroutine(payout.ShowPayOut (this));
    }

    public IEnumerator updatePayout ()
    {
        string winAmount = currencyManager.GetTotalWinAmount();
        string winMultiplier = GetWinMultiplier();
        CashOutUI.SetWinAmount(winAmount);
        CashOutUI.SetWinMultiplier(winMultiplier+"x");
        gamePlayManager.gamePlay.SetCashOutAmount(winAmount);
        CommandCenter.Instance.currencyMan_.CollectWinnings();
        yield return null;
    }

    public void resetPayout ()
    {
        string winAmount = "0.00";
        string winMultiplier = "0.00x";
        CashOutUI.SetWinAmount(winAmount);
        CashOutUI.UpdateWinAmount();
        CashOutUI.SetWinMultiplier(winMultiplier);
    }

    public float GetWinAmount ()
    {
        float winAmount = 0;
        if (CommandCenter.Instance.IsDemo())
        {
            int cardValue = (int)multipliersManager.GetSelectedMultiplierType();
            string betAmount = betManager.GetBetAmount();
            float parsedbetAmount;
            if (!float.TryParse(betAmount , out parsedbetAmount))
            {
                Debug.LogWarning($"Invalid multiplier format: '{betAmount}'");
                parsedbetAmount = 0f; // or any default value you consider safe
            }
            winAmount = cardValue * parsedbetAmount * multiplier();
        }
        else
        {

            winAmount = 0;
        }

        return winAmount;
    }

    public string GetWinMultiplier ()
    {
        if(CommandCenter.Instance.IsDemo())
        {
            return multipliersManager.GetMultiplier();
        }
        else
        {
            bool IsFirstTime = gamePlayManager.Get_IsFirstTime();
            bool IsSkip = gamePlayManager.Get_IsSkip();

            //if is firstTime or if is not first time && if skip or not
            GameState selectedGameState;
            string selectedSignature;

            if (IsFirstTime)
            {
                selectedGameState = apiManager.StartApi.gameResponse.game_state;
                selectedSignature = apiManager.StartApi.gameResponse.signature;
                Debug.Log("Using StartApi game_state & signature");
            }
            else if (IsSkip)
            {
                selectedGameState = apiManager.SkipApi.skipResponse.game_state;
                selectedSignature = apiManager.SkipApi.skipResponse.signature;
                Debug.Log("Using SkipApi game_state & signature");
            }
            else
            {
                selectedGameState = apiManager.guessApi.guessResponse.game_state;
                selectedSignature = apiManager.guessApi.guessResponse.signature;
                Debug.Log("Using guessResponse game_state & signature");
            }
            return selectedGameState.accumulated_win.ToString("N2");
        }
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
