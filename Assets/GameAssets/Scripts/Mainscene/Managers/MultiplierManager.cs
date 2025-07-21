using System.Linq;
using UnityEngine;
public enum MultiplierType
{
    None = 0,
    High,
    Same,
    Low,
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
    CardManager cardManager;
    public MultiplierType selectedMultiplier;
    public Multipliers Multipliers;
    public multiplierDetails [] multiplierDetailsList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardManager = CommandCenter.Instance.cardManager_;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetMultiplier ()
    {
        selectedMultiplier = MultiplierType.None;
    }

    public void RefreshMultipliers ()
    {
        var currentCard = cardManager.GetCurrentCardData();
        if (currentCard == null)
        {
           // Debug.LogWarning("Current card data is null.");
            return;
        }

        foreach (var multiDetails in multiplierDetailsList)
        {
            if (multiDetails.Rank != currentCard.cardRank)
                continue;

           // Debug.Log($"Rank : {multiDetails.Rank}");

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
                            textHelper.ManualRefresh(multiplierValue);
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

}
