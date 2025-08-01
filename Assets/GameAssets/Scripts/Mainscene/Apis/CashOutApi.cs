using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class CashOutRequest
{
    public string client_id;
    public string game_id;
    public string player_id;
    public string bet_id;
    public GameState game_state;
    public string signature;
}

[System.Serializable]
public class CashOutResponse
{
    public string status;
    public string message;
    public GameState game_state;
    public string signature;
}
public class CashOutApi : MonoBehaviour
{
    ApiManager apiMan;
    GamePlayManager gamePlayMan;
    public CashOutResponse cashOutResponse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
        gamePlayMan =  CommandCenter.Instance.gamePlayManager_;
    }

    public void CashOut ()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        bool IsFirstTime = gamePlayMan.Get_IsFirstTime();
        bool IsSkip = gamePlayMan.Get_IsSkip();

        CashOutRequest request = new CashOutRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.GetBetId() ,
            game_state = IsFirstTime ? apiMan.StartApi.gameResponse.game_state : IsSkip ? apiMan.SkipApi.skipResponse.game_state : apiMan.guessApi.guessResponse.game_state ,
            signature = IsFirstTime ? apiMan.StartApi.gameResponse.signature : IsSkip ? apiMan.SkipApi.skipResponse.signature : apiMan.guessApi.guessResponse.signature ,
        };

        string jsonData = JsonConvert.SerializeObject(request , settings);
        Debug.Log(jsonData);
        StartCoroutine(CashOutAction(jsonData));
    }

    IEnumerator CashOutAction (string jsonData)
    {
        string baseUrl = ConfigMan.Instance.Base_url;
        string ApiUrl = "https://b.api.ibibe.africa" + "/skip/hilo";
        using (UnityWebRequest webRequest = new UnityWebRequest(ApiUrl , "POST"))
        {
            byte [] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type" , "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                cashOutResponse = JsonUtility.FromJson<CashOutResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"Guess api successfully:{formattedOutput}");
            }
        }
    }
}
