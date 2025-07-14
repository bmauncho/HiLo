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
    public MultiplierType selectedMultiplier;
    public multiplierDetails [] multiplierDetailsList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
