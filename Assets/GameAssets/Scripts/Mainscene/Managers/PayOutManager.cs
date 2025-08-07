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

    public double GetWinAmount ()
    {
        if (CommandCenter.Instance.IsDemo())
        {
            int cardValue = (int)multipliersManager.GetSelectedMultiplierType();
            string betAmount = betManager.GetBetAmount();
            double parsedbetAmount;
            if (!double.TryParse(betAmount , out parsedbetAmount))
            {
                Debug.LogWarning($"Invalid multiplier format: '{betAmount}'");
                parsedbetAmount = 0f; // or any default value you consider safe
            }
           return cardValue * parsedbetAmount * multiplier();
        }
        else
        {
            if (gamePlayManager.gamePlay.isGamePlayActive) 
            {
                bool IsFirstTime = gamePlayManager.Get_IsFirstTime();
                bool IsSkip = gamePlayManager.Get_IsSkip();

                //if is firstTime or if is not first time && if skip or not
                GameState selectedGameState;
                GuessResult guess_result;

                switch (IsSkip)
                {
                    case true:
                        selectedGameState = apiManager.SkipApi.skipResponse.game_state;

                        //Debug.Log("Using SkipApi game_state & signature");

                        return selectedGameState.final_win;
                    case false:
                        switch (IsFirstTime)
                        {
                            case true:
                                selectedGameState = apiManager.StartApi.gameResponse.game_state;

                                //Debug.Log("Using StartApi game_state & signature");

                                return selectedGameState.final_win;
                            case false:
                                guess_result = apiManager.guessApi.guessResponse.guess_result;

                                //Debug.Log("Using guessResponse game_state & signature");

                                return guess_result.total_win_Amount;
                        }
                }
            }
            else
            {
                return apiManager.cashOutApi.cashOutResponse.final_win;
            }
        }
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

            switch (IsSkip)
            {
                case true:
                    selectedGameState = apiManager.SkipApi.skipResponse.game_state;

                    Debug.Log("Using SkipApi game_state & signature");

                    return selectedGameState.previous_winning_multiplier.ToString("N2");
                case false:
                    switch (IsFirstTime)
                    {
                        case true:
                            selectedGameState = apiManager.StartApi.gameResponse.game_state;

                            Debug.Log("Using StartApi game_state & signature");

                            return selectedGameState.previous_winning_multiplier.ToString("N2");
                        case false:
                            selectedGameState = apiManager.guessApi.guessResponse.game_state;

                            Debug.Log("Using guessResponse game_state & signature");

                            return selectedGameState.previous_winning_multiplier.ToString("N2");
                    }
            }
        }
    }

    public double multiplier ()
    {
        string multiplier = GetWinMultiplier().ToString();

        multiplier = multiplier.TrimEnd();

        double parsedMultiplier;
        if (!double.TryParse(multiplier , out parsedMultiplier))
        {
            Debug.LogWarning($"Invalid multiplier format: '{multiplier}'");
            parsedMultiplier = 0f; // or any default value you consider safe
        }
        return parsedMultiplier;
    }
}
