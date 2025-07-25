using System;
using UnityEngine;

public static class ProbabilityCalculator
{
    const int SuitsPerRank = 4;
    const int TotalRanks = 13;
    const int TotalCards = 52;

    public static int GetFavorableCardCount ( CardRanks currentCard , MultiplierType guessType )
    {
        int currentRank = (int)currentCard;
        int favorableRanks = 0;

        switch (guessType)
        {
            case MultiplierType.Low:
                favorableRanks = currentRank; // e.g., for NINE (8), ranks 0–7
                break;

            case MultiplierType.High:
                favorableRanks = TotalRanks - currentRank - 1;
                break;

            case MultiplierType.Same:
                favorableRanks = 1;
                break;
        }

        int favorableCards = favorableRanks * SuitsPerRank;
        return favorableCards;
    }

    public static float GetProbability(int favorable )
    {
        int remaining = TotalCards - 1;

        if (favorable == 0) return 0f;

        float probability = (float)favorable / remaining;

        return probability;
    }

    public static float GetMultiplier ( int favorable ,float baseMultiplier, float houseEdge = 0.1f )
    {
        float probability = GetProbability(favorable);
        if (probability <= 0f) return 0f;

        float fairMultiplier = 1f / probability;
        return fairMultiplier * baseMultiplier *( 1f - houseEdge );
    }
}
