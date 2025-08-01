using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class SkipRequest
{
    public string client_id;
    public string game_id;
    public string player_id;
    public string bet_id;
    public GameState game_state;
    public string signature;
}
[System.Serializable]
public class SkipResponse
{
    public string status;
    public string message;
    public GameState game_state;
    public GuessResult guess_result;
    public BetOptions [] bet_options;
    public string signature;
}
public class SkipApi : MonoBehaviour
{
    ApiManager apiMan;
    public SkipResponse skipResponse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
    }

    public void Skip ()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        bool IsFirstTime = true;//logic needed
        bool IsSkip = false;//logic needed

        SkipRequest skipRequest = new SkipRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.GetBetId() ,
            game_state = IsFirstTime ? apiMan.StartApi.gameResponse.game_state : IsSkip ? skipResponse.game_state : apiMan.guessApi.guessResponse.game_state ,
            signature = IsFirstTime ? apiMan.StartApi.gameResponse.signature : IsSkip ? skipResponse.signature : apiMan.guessApi.guessResponse.signature ,
        };

        string jsonData = JsonConvert.SerializeObject(skipRequest , settings);
        Debug.Log(jsonData);
        StartCoroutine(skipAction(jsonData));
    }

    IEnumerator skipAction(string jsonData )
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
                skipResponse = JsonUtility.FromJson<SkipResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"Guess api successfully:{formattedOutput}");
            }
        }
    }

}
