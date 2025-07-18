using TMPro;
using UnityEngine;

public class Bet : MonoBehaviour
{
    public string BetAmount = string.Empty;
    public TMP_Text betAmountText;
    public BetButton IncreaseBtn;
    public GameObject IncreaseBtnMask;
    public BetButton DecreaseBtn;
    public GameObject DecreaseBtnMask;

    public void SetBetAmount(string amount )
    {
        BetAmount = amount;
        UpdateBetAmount(amount);
    }

    void UpdateBetAmount (string input)
    {
        betAmountText.GetComponent<TextHelper>().ManualRefresh(input);
    }
}
