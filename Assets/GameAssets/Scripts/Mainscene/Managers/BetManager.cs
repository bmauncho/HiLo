using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum BetType { Increase, Decrease }
public class BetManager : MonoBehaviour
{
    public List<string> BetAmounts = new List<string> { "1" , "2" , "3" , "5" , "10" , "20" , "30" , "50" , "100" , "200" , "300" , "500" };
    public int betIndex = 3;
    public string betAmount = "";
    public Bet Bet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //refresh();
        betIndex = 0;
        betAmount = BetAmounts[betIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpBetValues ()
    {
        if (CommandCenter.Instance != null && !CommandCenter.Instance.IsDemo())
        {
            if (ConfigMan.Instance.BetValues != null || ConfigMan.Instance.BetValues.Length > 0)
            {
                return;
            }
            BetAmounts = new List<string>(ConfigMan.Instance.BetValues.ToList());
        }
        refresh();
    }

    void refresh ()
    {
        betAmount = BetAmounts [betIndex];
        Bet.SetBetAmount(betAmount);
    }

    public void IncreaseBetAmount_click()
    {
        if (betIndex < BetAmounts.Count - 1)
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
        if (betIndex < BetAmounts.Count - 1)
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
        return betIndex >= BetAmounts.Count-1;
    }

    public bool IsLowestBetAmount ()
    {
        return betIndex <= 0;
    }
}


