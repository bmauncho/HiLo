using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
[System.Serializable]
public class GuessRequest
{
    public string client_id;
    public string game_id;
    public string player_id;
    public string bet_id;
    public GameState game_state;
    public string bet_choice;
    public string signature;
}

[System.Serializable]
public class GuessResponse
{
    public string status;
    public string message;
    public GameState game_state;
    public GuessResult guess_result;
    public BetOptions [] bet_options;
    public string signature;
}

[System.Serializable]
public class GuessResult
{
    public string success;
    public string next_card;
    public int next_value;
    public float payout_multiple;
    public bool was_correct;
    public bool forced;

}
public class GuessApi : MonoBehaviour
{
    ApiManager apiMan;
    MultiplierManager multipliersMan;
    public GuessResponse guessResponse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
        multipliersMan = CommandCenter.Instance.multiplierManager_;
    }

    [ContextMenu("Guess")]
    public void Guess ()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        bool IsFirstTime = true;//logic needed
        bool IsSkip = false;//logic needed

        //if is firstTime or if is not first time && if skip or not
        GuessRequest request = new GuessRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.GetBetId() ,
            game_state = IsFirstTime ? apiMan.StartApi.gameResponse.game_state : IsSkip ? guessResponse.game_state : guessResponse.game_state,
            bet_choice= multipliersMan.selectedMultiplier.ToString(),
            signature = IsFirstTime ? apiMan.StartApi.gameResponse.signature: IsSkip ? guessResponse.signature: guessResponse.signature,
        };
        string jsonData = JsonConvert.SerializeObject(request , settings);
        Debug.Log(jsonData);
        StartCoroutine(GuessAction(jsonData));
    }

    IEnumerator GuessAction(string jsonData )
    {
        string baseUrl = ConfigMan.Instance.Base_url;
        string ApiUrl = "https://b.api.ibibe.africa" + "/guess/hilo";
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
                guessResponse = JsonUtility.FromJson<GuessResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"Guess api successfully:{formattedOutput}");
            }
        }
    }
}
