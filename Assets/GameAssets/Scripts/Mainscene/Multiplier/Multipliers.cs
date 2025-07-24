using UnityEngine;

public class Multipliers : MonoBehaviour
{
    public Multiplier [] multipliers;

    public void ToggleMultiplier ( CardData card )
    {
        foreach (Multiplier p in multipliers)
        {
            switch (card.cardRank)
            {
                case CardRanks.ACE:
                    p.SetInteractable(p.multiplier != MultiplierType.Low);
                    break;
                case CardRanks.KING:
                    p.SetInteractable(p.multiplier != MultiplierType.High);
                    break;
                default:
                    p.enableBtn();
                    break;
            }
        }
    }

}
