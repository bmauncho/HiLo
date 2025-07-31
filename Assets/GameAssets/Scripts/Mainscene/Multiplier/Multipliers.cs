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

    public void ToggleMultiplierType ( CardData card )
    {
        bool isAce = card.cardRank == CardRanks.ACE;
        bool isKing = card.cardRank == CardRanks.KING;

        foreach (Multiplier p in multipliers)
        {
            switch (p.multiplier)
            {
                case MultiplierType.High:
                    if (!isKing)
                        p.multiplier = MultiplierType.HighOrSame;
                    break;

                case MultiplierType.HighOrSame:
                    if (isKing)
                    {
                        p.multiplier = MultiplierType.High;
                    }
                    else
                    {
                        p.multiplier = MultiplierType.HighOrSame;
                    }
                    break;

                case MultiplierType.Low:
                    if (!isAce)
                        p.multiplier = MultiplierType.LowOrSame;
                    break;

                case MultiplierType.LowOrSame:
                    if (isAce)
                    {
                        p.multiplier = MultiplierType.Low;
                    }
                    else
                    {
                        p.multiplier = MultiplierType.LowOrSame;
                    }
                    break;
                case MultiplierType.Same:
                    p.multiplier = MultiplierType.Same;
                    break;

                default:
                    if (isKing)
                        p.multiplier = MultiplierType.High;
                    else if (isAce)
                        p.multiplier = MultiplierType.Low;
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
