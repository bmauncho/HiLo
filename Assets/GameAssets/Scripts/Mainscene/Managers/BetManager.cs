using UnityEngine;
public enum BetType { Increase, Decrease }
public class BetManager : MonoBehaviour
{
    private string [] BetAmounts = { "1" , "2" , "3" , "5" , "10" , "20" , "30" , "50" , "100" , "200" , "300" , "500" };
    public int betIndex = 3;
    public string betAmount = "";
    public Bet Bet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void refresh ()
    {
        betAmount = BetAmounts [betIndex];
        Bet.SetBetAmount(betAmount);
    }

    public void IncreaseBetAmount_click()
    {
        if (betIndex < BetAmounts.Length - 1)
        {
            betIndex++;
            betAmount = BetAmounts [betIndex];
            Bet.SetBetAmount (betAmount);
        }
    }

    public void DecreaseBetAmount_Click()
    {
        if (betIndex > 0)
        {
            betIndex--;
            betAmount = BetAmounts [betIndex];
            Bet.SetBetAmount(betAmount);
        }
    }

    public void IncreaseBetAmount_Hold ()
    {
        if (betIndex < BetAmounts.Length - 1)
        {
            betIndex++;
            betAmount = BetAmounts [betIndex];
            Bet.SetBetAmount(betAmount);
        }
    }

    public void DecreaseBetAmount_Hold ()
    {
        if (betIndex > 0)
        {
            betIndex--;
            betAmount = BetAmounts [betIndex];
            Bet.SetBetAmount(betAmount);
        }
    }

    public string GetBetAmount ()
    {
        return betAmount;
    }

    public bool IsHighestBetAmount ()
    {
        return betIndex >= BetAmounts.Length-1;
    }

    public bool IsLowestBetAmount ()
    {
        return betIndex <= 0;
    }
}


