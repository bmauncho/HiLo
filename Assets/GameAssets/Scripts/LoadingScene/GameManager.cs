using System.Collections;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class PlayerInfo
{
    public string id;
    public string names;
    public string msisdn;
    public string account_number;
    public string email_address;
    public string is_blacklisted;
    public string wallet_balance;
    public string created_at;
    public string last_bet_date;
    public string last_win_date;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private bool isDataFetched = false;
    [SerializeField] private bool isDemo = false;
    [SerializeField] private string Player_Id;
    [SerializeField] private string Game_Id;
    [SerializeField] private string Client_id;
    [SerializeField] private string CashAmount = string.Empty;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private TMP_Text [] TransactionsText;
    string transaction = string.Empty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start ()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void FetchConfigData ()
    {
        //Debug.Log("FetchingConfig");
        if (ConfigMan.Instance.ReceivedConfigs)
        {
            if (!string.IsNullOrEmpty(ConfigMan.Instance.PlayerId))
            {
                Player_Id = ConfigMan.Instance.PlayerId;
            }

            if (!string.IsNullOrEmpty(ConfigMan.Instance.GameId))
            {
                Game_Id = ConfigMan.Instance.GameId;
            }

            if (!string.IsNullOrEmpty(ConfigMan.Instance.ClientId))
            {
                Client_id = ConfigMan.Instance.ClientId;
            }

            if (ConfigMan.Instance.IsDemo)
            {
                CashAmount = "2000";
            }
            //Debug.Log($"is Demo {ConfigMan.Instance.IsDemo}");
            isDemo = ConfigMan.Instance.IsDemo;
            FetchPlayerInfo();
        }
        else
        {
            //Debug.Log($"is Demo {ConfigMan.Instance.IsDemo}");
            isDemo = ConfigMan.Instance.IsDemo;
            FetchPlayerInfo();
        }
    }

    public void FetchPlayerInfo ()
    {
        isDataFetched = false;
        StartCoroutine(_FetchPlayerInfo(ConfigMan.Instance.Base_url + "/api/v1/customer/details?customer_id=" + Player_Id));
    }

    private IEnumerator _FetchPlayerInfo ( string url )
    {
        using (UnityWebRequest www = UnityWebRequestHelper.GetWithTimestamp(url))
        {
            www.useHttpContinue = false;
            www.SetRequestHeader("Cache-Control" , "no-cache, no-store, must-revalidate");
            www.SetRequestHeader("Pragma" , "no-cache");
            www.SetRequestHeader("Expires" , "0");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Received: " + www.downloadHandler.text);
                playerInfo = JsonUtility.FromJson<PlayerInfo>(www.downloadHandler.text);
                CashAmount = playerInfo.wallet_balance;
                isDataFetched = true;
            }
            else
            {
                isDataFetched = true;
                Debug.Log("Error: " + www.error);
                CashAmount = "2000";
                PromptManager.Instance.ShowErrorPrompt(
                    www.responseCode.ToString() , 
                    www.result.ToString() + www.error.ToString());
            }
        }
    }

    public void AddTransactionText ( TMP_Text theText )
    {
        if (TransactionsText == null)
        {
            TransactionsText = new TMP_Text [] { theText };
        }
        else
        {
            int length = TransactionsText.Length;
            TMP_Text [] newArray = new TMP_Text [length + 1];
            for (int i = 0 ; i < length ; i++)
            {
                newArray [i] = TransactionsText [i];
            }
            newArray [length] = theText;
            TransactionsText = newArray;
        }
    }

    public void ShowTransaction ( string thetrans )
    {
        transaction = "No.";
        thetrans = transaction + " 748838" + thetrans;
        for (int i = 0 ; i < TransactionsText.Length ; i++)
        {
            TransactionsText [i].text = thetrans;
        }
    }

    public bool IsDataFetched ()
    {
        return isDataFetched;
    }

    public string GetPlayerId ()
    {
        return Player_Id;
    }

    public string GetGameId ()
    {
        return Game_Id;
    }

    public string GetClientId ()
    {
        return Client_id;
    }
    public string GetCashAmount ()
    {
        return CashAmount;
    }
    public bool IsDemo ()
    {
        return isDemo;
    }
}
