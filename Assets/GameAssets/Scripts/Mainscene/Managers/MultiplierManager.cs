using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum MultiplierType
{
    None = 0,
    higher,
    higher_or_same,
    same,
    lower,
    lower_or_same,
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
[System.Serializable]
public class MultiplierButtonDetails
{
    public MultiplierType multiplierType;
    public ImageTranslationInfo [] translationInfo;
}

public class MultiplierManager : MonoBehaviour
{
    GamePlayManager gamePlayManager;
    CardManager cardManager;
    ApiManager apiManager;
    public MultiplierType prevSelectedMultiplier;
    public MultiplierType selectedMultiplier;
    public Multipliers Multipliers;
    public MultiplierButtonDetails [] buttonDetails;
    public multiplierDetails [] multiplierDetailsList;

    bool Init=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start ()
    {
        cardManager = CommandCenter.Instance.cardManager_;
        gamePlayManager = CommandCenter.Instance.gamePlayManager_;
        apiManager = CommandCenter.Instance.apiManager_;
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public string GetMultiplier ()
    {
        foreach (var multiplier in Multipliers.multipliers)
        {
            if (multiplier.multiplier == selectedMultiplier)
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

    public void SetSelectedMultiplier ( MultiplierType multiplierType )
    {
        prevSelectedMultiplier = selectedMultiplier;
        selectedMultiplier = multiplierType;
    }

    public void enableGuessMask ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.EnableMask();
        }
    }

    public void disableGuessMask ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.DisableMask();
        }
    }

    public void enableGuessBtns ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.enableBtn();
        }
    }

    public void disableGuessBtns ()
    {
        foreach (Multiplier multiplier in Multipliers.multipliers)
        {
            multiplier.disableBtn();
        }
    }


    public void RefreshMultipliers ()
    {
        if (CommandCenter.Instance.IsDemo())
        {
            DemoRefresh();
        }
        else
        {
            LiveRefresh();
        }

    }

    void DemoRefresh ()
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
                                if (multiplierValue != "")
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

    void LiveRefresh ()
    {
        bool IsFirstTime = gamePlayManager.Get_IsFirstTime();
        bool IsSkip = gamePlayManager.Get_IsSkip();
        bool IsGameStarted = gamePlayManager.IsGameStarted();

        BetOptions [] betOptions;

        switch (Init)
        {
            case true:
                switch (IsSkip)
                {
                    case true:
                        switch (IsGameStarted)
                        {
                            case true:
                                betOptions = apiManager.SkipApi.skipResponse.bet_options;
                                break;
                            case false:
                                betOptions = apiManager.previewSkipApi.response.bet_options;
                                break;
                        }

                        break;
                    case false:
                        switch (IsFirstTime)
                        {
                            case true:
                                betOptions = apiManager.StartApi.gameResponse.bet_options;
                                break;
                            case false:
                                betOptions = apiManager.guessApi.guessResponse.bet_options;
                                break;
                        }
                        break;
                }
                break;
            case false:
                betOptions = GameManager.Instance.previewApi.response.bet_options;
                Init = true;
                break;
        }

        foreach (var multiplier in Multipliers.multipliers)
        {
            var matchingMultiplier = betOptions
                    .FirstOrDefault(m => parsetoEnum(m.id) == multiplier.multiplier);

            if (matchingMultiplier != null)
            {
                string multiplierValue = matchingMultiplier.multiplier.ToString();
                //Debug.Log(multiplierValue);
                if (multiplier.multiplierText != null)
                {
                    TextHelper textHelper = multiplier.multiplierText.GetComponent<TextHelper>();
                    if (textHelper != null)
                    {
                        if (!string.IsNullOrEmpty(multiplierValue))
                        {
                            if (multiplierValue != "")
                            {
                                multiplierValue = multiplierValue + "x";
                            }
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
                }
                else
                {
                    Debug.LogWarning("multiplierText is null.");
                }
            }
        }
    }


    public List<MultiplierConfig> GetCurrentMultipliers ( bool isSkip = false )
    {
        if (CommandCenter.Instance.IsDemo())
        {
            return GetDemoMultipliers(isSkip);
        }
        else
        {
            return GetLiveMultipliers(isSkip);
        }

    }

    public List<MultiplierConfig> GetDemoMultipliers ( bool isSkip = false )
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
                // Skip 'same' type multipliers if skipping is enabled
                if (isSkip && multi.multiplierType == MultiplierType.same)
                    continue;

                bool isKing = multiDetails.Rank == CardRanks.KING && multi.multiplierType == MultiplierType.higher;
                bool isAce = multiDetails.Rank == CardRanks.ACE && multi.multiplierType == MultiplierType.lower;

                double multiplierValue = 0;

                string multiplierValueString = string.Empty;

                if (isKing || isAce)
                {
                    multiplierValueString = string.Empty;
                }
                else
                {
                   
                    if (!double.TryParse(multi.Multiplier , out double parsedMultiplier))
                    {
                        Debug.LogWarning($"Invalid multiplier format: '{multi.Multiplier}'");
                        parsedMultiplier = 0f; // or any default value you consider safe
                    }


                    multiplierValue = ProbabilityCalculator.GetMultiplier(GetPrevSelectedMultiplier(),parsedMultiplier);
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

    public List<MultiplierConfig> GetLiveMultipliers( bool isSkip = false )
    {
        CardRanks currentCard = cardManager.GetCurrentCardData().cardRank;
        MultiplierType mutiplierType = selectedMultiplier;
        List<MultiplierConfig> temp = new List<MultiplierConfig>();
        bool IsFirstTime = gamePlayManager.Get_IsFirstTime();
        bool IsGameStarted = gamePlayManager.IsGameStarted();
        BetOptions [] betOptions;

        switch (Init)
        {
            case true:
                switch (isSkip)
                {
                    case true:
                        switch (IsGameStarted)
                        {
                            case true:
                                betOptions = apiManager.SkipApi.skipResponse.bet_options;
                                break;
                            case false:
                                betOptions = apiManager.previewSkipApi.response.bet_options;
                                break;
                        }
                        
                        break;
                    case false:
                        switch (IsFirstTime)
                        {
                            case true:
                                betOptions = apiManager.StartApi.gameResponse.bet_options;
                                break;
                            case false:
                                betOptions = apiManager.guessApi.guessResponse.bet_options;
                                break;
                        }
                        break;
                }
                break;
            case false:
                betOptions = GameManager.Instance.previewApi.response.bet_options;
                Init = true;
                break;
        }

        foreach (var multiplier in Multipliers.multipliers)
        {
            var matchingMultiplier = betOptions
                    .FirstOrDefault(m => parsetoEnum(m.id) == multiplier.multiplier);
            string multiplierValueString = string.Empty;
            if (matchingMultiplier != null)
            {
                string multiplierValue = matchingMultiplier.multiplier.ToString();
                CardData cardData = cardManager.GetCurrentCardData();

                multiplierValueString = multiplierValue;

                temp.Add(new MultiplierConfig
                {
                    multiplierType = parsetoEnum(matchingMultiplier.id) ,
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

    public MultiplierButtonDetails [] ButtonDetails ()
    {
        return buttonDetails;
    }

    public MultiplierType parsetoEnum ( string multiplier )
    {
        // Replace spaces with underscores
        multiplier = multiplier.Replace(" " , "_");

        if (Enum.TryParse(multiplier , true , out MultiplierType result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning($"Failed to parse {multiplier}, returning default value");
            return MultiplierType.None;
        }
    }

    public double GetPrevSelectedMultiplier ()
    {
        CardData data = cardManager.GetPrevCardData();
        string multiplierValueString = string.Empty;
        if (prevSelectedMultiplier == MultiplierType.None)
        {
            prevSelectedMultiplier = selectedMultiplier;
        }

        foreach (var multiplier in multiplierDetailsList)
        {
            if(multiplier.Rank == data.cardRank)
            {
                var matchingMultiplier = multiplier.multipliers.
                FirstOrDefault(m => m.multiplierType == prevSelectedMultiplier);

                if (matchingMultiplier != null)
                {
                    multiplierValueString = matchingMultiplier.Multiplier;
                }
                else
                {
                    throw new Exception($"matching multiplier is null");
                }
                    break;
            }
        }

        multiplierValueString = multiplierValueString.Trim('x');

        if(!double.TryParse(multiplierValueString,out double prevMultiplier))
        {
            throw new Exception($"could not parse string {multiplierValueString} to double ");
        }

        return prevMultiplier;
    }

}
