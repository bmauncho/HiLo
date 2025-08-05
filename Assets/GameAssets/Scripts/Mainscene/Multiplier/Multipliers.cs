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
                    p.SetInteractable(p.multiplier != MultiplierType.lower_or_same);
                    break;
                case CardRanks.KING:
                    p.SetInteractable(p.multiplier != MultiplierType.higher_or_same);
                    break;
                default:
                    p.enableBtn();
                    break;
            }
        }
    }

    public void ToggleMultiplierType ( CardData card )
    {
        bool isAce = card.cardRank == CardRanks.ACE;
        bool isKing = card.cardRank == CardRanks.KING;

        foreach (Multiplier p in multipliers)
        {
            switch (p.multiplier)
            {
                case MultiplierType.higher:
                case MultiplierType.higher_or_same:
                    if (isKing)
                        p.multiplier = MultiplierType.higher_or_same;
                    else if (isAce)
                        p.multiplier = MultiplierType.higher;
                    else
                        p.multiplier = MultiplierType.higher_or_same;
                    break;

                case MultiplierType.lower:
                case MultiplierType.lower_or_same:
                    if (!isAce && isKing)
                        p.multiplier = MultiplierType.lower;
                    else 
                        p.multiplier = MultiplierType.lower_or_same;
                    break;

                case MultiplierType.same:
                    p.multiplier = MultiplierType.same;
                    break;
            }
        }
    }


    public void UpdateText ()
    {
        MultiplierButtonDetails [] ButtonDetails = 
            CommandCenter.Instance.multiplierManager_.ButtonDetails();
        foreach (Multiplier p in multipliers)
        {
            foreach(MultiplierButtonDetails button in ButtonDetails)
            {
                if(p.multiplier == button.multiplierType)
                {
                    p.SetTypeText(button.Type);
                }
            }
        }
    }

}
