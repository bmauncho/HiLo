using System;
using UnityEngine;

public static class ProbabilityCalculator
{
    const int SuitsPerRank = 4;
    const int TotalRanks = 13;
    const int TotalCards = 52;
    const double empirical_constant = 0.0058795465029;
    public static double GetBonus (double currentbaseMultiplier )
    { 
        return 1f + ( currentbaseMultiplier * empirical_constant );
    }

    public static double GetMultiplier (double prevSelectedMultiplier, double currentBaseMultiplier)
    {
        return ( prevSelectedMultiplier * currentBaseMultiplier * GetBonus(currentBaseMultiplier) );
    }
}
