using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class UpdateBetRequest
{
    public string bet_id;
    public string amount_won;
    public string client_id;
}

[System.Serializable]
public class UpdateBetResponse
{
    public float status_code = 0;
    public string message = "";
    public string bet_id;
    public string amount_won;
    public string new_wallet_balance;
    public string status;
}
public class UpdateBet : MonoBehaviour
{
    //private const string ApiUrl = "https://admin-api.ibibe.africa/api/v1/bet/update_bet";
    [Header("Api references")]
    public UpdateBetResponse updateBetResponse;
    [Header("Api Values")]
    public string AmountWon;
    public double new_wallet_balance;
    public bool isUpdated = false;
    [ContextMenu("Update Bet")]
    public void UpdateTheBet ()
    {
        isUpdated = false;
        UpdateBetRequest updateBetRequest = new UpdateBetRequest
        {
            bet_id = CommandCenter.Instance.apiManager_.placeBet.bet_id,
            amount_won = AmountWon ,
            client_id = CommandCenter.Instance.apiManager_.GetClientId() ,
        };
        string jsonData = JsonUtility.ToJson(updateBetRequest , true);
        Debug.Log("Update Bet payload" + jsonData);
        StartCoroutine(UpdateBetCoroutine(jsonData));
    }

    private IEnumerator UpdateBetCoroutine ( string jsonData )
    {
        string ApiUrl = ConfigMan.Instance.Base_url + "/api/v1/update_bet";
        using (UnityWebRequest webRequest = new UnityWebRequest(ApiUrl , "POST"))
        {
            byte [] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type" , "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.result);
                isUpdated = true;
                PromptManager.Instance.ShowErrorPrompt(webRequest.result.ToString() ,webRequest.error);
            }
            else
            {
                updateBetResponse = JsonUtility.FromJson<UpdateBetResponse>(webRequest.downloadHandler.text);
                var parsedJson = JToken.Parse(webRequest.downloadHandler.text);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log("UpdateBet Response: " + formattedOutput);
                //AmountWon = updateBetResponse.amount_won.ToString();
                double cashAmount = double.Parse(updateBetResponse.new_wallet_balance);
                new_wallet_balance = cashAmount;
                isUpdated = true;
            }
        }
    }

    public void SetAmountWon ( double Amount )
    {
        AmountWon = Amount.ToString();
    }
}
