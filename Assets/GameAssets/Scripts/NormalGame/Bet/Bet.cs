using TMPro;
using UnityEngine;

public class Bet : MonoBehaviour
{
    public string BetAmount = string.Empty;
    public TMP_Text betAmountText;
    public BetBtnMask IncreaseBtn;
    public BetBtnMask DecreaseBtn;
    public void SetBetAmount(string amount )
    {
        BetAmount = amount;
        UpdateBetAmount(amount);
        CommandCenter.Instance.apiManager_.GetBetAmount(amount);
    }

    void UpdateBetAmount (string input)
    {
        betAmountText.GetComponent<TextHelper>().ManualRefresh(input);
    }
}
