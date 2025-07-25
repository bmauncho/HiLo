using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum MultiplierType
{
    None = 0,
    High,
    HighOrSame,
    Same,
    Low,
    LowOrSame,
}
[System.Serializable]
public class multiplierDetails
{
    public CardRanks Rank;
    public MultiplierConfig [] multipliers;
}

[System.Serializable]
public class MultiplierConfig
{
    public MultiplierType multiplierType;
    public string Multiplier = "";
}
public class MultiplierManager : MonoBehaviour
{
    GamePlayManager gamePlayManager;    
    CardManager cardManager;
    public MultiplierType selectedMultiplier;
    public Multipliers Multipliers;
    public multiplierDetails [] multiplierDetailsList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardManager = CommandCenter.Instance.cardManager_;
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetMultiplier ()
    {
        foreach(var multiplier in Multipliers.multipliers)
        {
            if(multiplier.multiplier == selectedMultiplier)
            {
                return multiplier.TheMultiplier;
            }
        }

        return string.Empty;
    }

    public MultiplierType GetSelectedMultiplierType ()
    {
        return selectedMultiplier;
    }

    public void resetMultiplier ()
    {
        selectedMultiplier = MultiplierType.None;
    }

    public void SetSelectedMultiplier(MultiplierType multiplierType )
    {
        selectedMultiplier = multiplierType;
    }

    public void enableGuessMask ()
    {
        foreach( Multiplier multiplier in Multipliers.multipliers )
        {
            multiplier.EnableMask ();
        }
    }

    public void disableGuessMask ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.DisableMask ();
        }
    }

    public void enableGuessBtns ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.enableBtn ();
        }
    }

    public void disableGuessBtns ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.disableBtn ();
        }
    }


    public void RefreshMultipliers ()
    {
        var currentCard = cardManager.GetCurrentCardData();
        if (currentCard == null)
        {
           Debug.LogWarning("Current card data is null.");
            return;
        }

        foreach (var multiDetails in multiplierDetailsList)
        {
            if (multiDetails.Rank != currentCard.cardRank)
                continue;

            //Debug.Log($"Rank : {multiDetails.Rank}");

            foreach (var multi in multiDetails.multipliers)
            {
                // Try find a matching multiplier only once
                var matchingMultiplier = Multipliers.multipliers
                    .FirstOrDefault(m => m.multiplier == multi.multiplierType);

                if (matchingMultiplier != null)
                {
                    string multiplierValue = multi.Multiplier;
                    //Debug.Log($"multiplier type - {multi.multiplierType} : multiplier value - {multiplierValue}");

                    matchingMultiplier.SetText(multiplierValue);

                    if (matchingMultiplier.multiplierText != null)
                    {
                        TextHelper textHelper = matchingMultiplier.multiplierText.GetComponent<TextHelper>();
                        if (textHelper != null)
                        {
                            if (!string.IsNullOrEmpty(multiplierValue))
                            {
                                if(multiplierValue != "")
                                {
                                    multiplierValue = multiplierValue + "x";
                                }
                                textHelper.ManualRefresh(multiplierValue);
                                if (gamePlayManager.IsGameStarted())
                                {
                                    matchingMultiplier.enableBtn();
                                }
                                else
                                {
                                    matchingMultiplier.disableBtn();
                                }
                            }
                            else
                            {
                                textHelper.ManualRefresh(multiplierValue);
                                matchingMultiplier.disableBtn();
                            }
                        }
                        else
                        {
                            Debug.LogWarning("TextHelper component not found.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("multiplierText is null.");
                    }
                }
            }

            // Break after match since Rank is unique
            break;
        }
    }


    public List<MultiplierConfig> GetCurrentMultipliers (bool isSkip =false)
    {
        CardRanks currentCard = cardManager.GetCurrentCardData().cardRank;
        MultiplierType mutiplierType = selectedMultiplier;
        List<MultiplierConfig> temp = new List<MultiplierConfig>();
        foreach (var multiDetails in multiplierDetailsList)
        {
            if (multiDetails.Rank != currentCard)
                continue;

            foreach (var multi in multiDetails.multipliers)
            {
                // Skip 'Same' type multipliers if skipping is enabled
                if (isSkip && multi.multiplierType == MultiplierType.Same)
                    continue;

                bool isKing = multiDetails.Rank == CardRanks.KING && multi.multiplierType == MultiplierType.High;
                bool isAce = multiDetails.Rank == CardRanks.ACE && multi.multiplierType == MultiplierType.Low;

                float multiplierValue = 0;

                string multiplierValueString = string.Empty;
                int favourable = ProbabilityCalculator.GetFavorableCardCount(currentCard , multi.multiplierType);

                if (isKing || isAce)
                {
                    multiplierValueString = string.Empty;
                }
                else
                {
                    float parsedMultiplier;
                    if (!float.TryParse(multi.Multiplier , out parsedMultiplier))
                    {
                        Debug.LogWarning($"Invalid multiplier format: '{multi.Multiplier}'");
                        parsedMultiplier = 0f; // or any default value you consider safe
                    }


                    multiplierValue = ProbabilityCalculator.GetMultiplier(favourable , parsedMultiplier);
                }

                multiplierValueString = multiplierValue == 0 ? string.Empty : multiplierValue.ToString("F2");
    
                  
                temp.Add(new MultiplierConfig
                {
                    multiplierType = multi.multiplierType ,
                    Multiplier = multiplierValueString ,
                });
            }
        }

        return temp;
    }

    public void refreshMultplierValues ( List<MultiplierConfig> config )
    {
        foreach (var multiplier in Multipliers.multipliers)
        {
            foreach (MultiplierConfig configItem in config)
            {
                if (multiplier.multiplier == configItem.multiplierType)
                {
                    string multiplierValue = configItem.Multiplier;
                    TextHelper textHelper = multiplier.multiplierText.GetComponent<TextHelper>();
                    if (textHelper != null)
                    {
                        if(multiplierValue != "")
                        {
                            multiplierValue = multiplierValue + "x";
                        }

                        if (!string.IsNullOrEmpty(multiplierValue))
                        {
                            textHelper.ManualRefresh(multiplierValue);
                            if (gamePlayManager.IsGameStarted())
                            {
                                multiplier.enableBtn();
                            }
                            else
                            {
                                multiplier.disableBtn();
                            }
                        }
                        else
                        {
                            textHelper.ManualRefresh(multiplierValue);
                            multiplier.disableBtn();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("TextHelper component not found.");
                    }

                    break; // break only if a match was found
                }
            }
        }
    }

}
