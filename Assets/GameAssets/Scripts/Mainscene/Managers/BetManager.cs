using DG.Tweening;
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
        if (CommandCenter.Instance != null)
        {
            if (ConfigMan.Instance.BetValues == null || ConfigMan.Instance.BetValues.Length <= 0)
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

        Bet.IncreaseBtn.transform.DOPunchScale(-new Vector3(0.2f , 0.2f , 0.2f) , .25f , 0 , 1).OnComplete(() =>
        {
            Bet.IncreaseBtn.transform.localScale = Vector3.one;
        });

        UpdateBetButtons();
    }

    public void DecreaseBetAmount_Click()
    {
        if (betIndex > 0)
        {
            betIndex--;
            betAmount = BetAmounts [betIndex];
            Bet.SetBetAmount(betAmount);

        }

        Bet.DecreaseBtn.transform.DOPunchScale(-new Vector3(0.2f , 0.2f , 0.2f) , .25f , 0 , 1).OnComplete(() =>
        {
            Bet.DecreaseBtn.transform.localScale = Vector3.one;
        });

        UpdateBetButtons();
    }

    private void UpdateBetButtons ()
    {
        // If at MIN value (index == 0)
        if (betIndex <= 0)
        {
            Bet.DecreaseBtn.ActivateMask();      // Deactivate decrease
            Bet.IncreaseBtn.DeactivateMask();    // Activate increase
            return;
        }

        // If at MAX value (index == last)
        if (betIndex >= BetAmounts.Count - 1)
        {
            Bet.DecreaseBtn.DeactivateMask();    // Activate decrease
            Bet.IncreaseBtn.ActivateMask();      // Deactivate increase
            return;
        }

        // Else: BOTH ACTIVE
        Bet.DecreaseBtn.DeactivateMask();        // Active
        Bet.IncreaseBtn.DeactivateMask();        // Active
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


