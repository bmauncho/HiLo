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
    GamePlayManager gamePlayMan;
    public SkipResponse skipResponse;
    public bool IsSkiped;
    bool skipInit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
        gamePlayMan = CommandCenter.Instance.gamePlayManager_;
    }

    public void Skip ()
    {
        IsSkiped = false;
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;


        bool IsFirstTime = gamePlayMan.Get_IsFirstTime();
        bool IsSkip = gamePlayMan.Get_IsSkip();

        //if is firstTime or if is not first time && if skip or not
        GameState selectedGameState = null;
        string selectedSignature = "";

        if (IsFirstTime)
        {
            selectedGameState = apiMan.StartApi.gameResponse.game_state;
            selectedSignature = apiMan.StartApi.gameResponse.signature;
            Debug.Log("Using StartApi game_state & signature");
        }
        else
        {
            selectedGameState = apiMan.guessApi.guessResponse.game_state;
            selectedSignature = apiMan.guessApi.guessResponse.signature;
            Debug.Log("Using guessResponse game_state & signature");
        }

        Debug.Log($"Selected GameState: {selectedGameState}");
        Debug.Log($"Selected Signature: {selectedSignature}");

        SkipRequest skipRequest = new SkipRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.GetBetId() ,
            game_state = selectedGameState,
            signature = selectedSignature ,
        };

        string jsonData = JsonConvert.SerializeObject(skipRequest , settings);
        Debug.Log($"skip api request:{jsonData}");
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
                IsSkiped = true;
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                skipResponse = JsonConvert.DeserializeObject<SkipResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"skip api response:{formattedOutput}");
                IsSkiped = true;
            }
        }
    }

    public void ResetSkipInit ()
    {
        IsSkiped = false;
    }

}
