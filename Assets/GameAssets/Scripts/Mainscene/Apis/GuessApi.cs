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
    public double payout_multiple;
    public double total_win_Amount;
    public bool was_correct;
    public bool forced;

}
public class GuessApi : MonoBehaviour
{
    ApiManager apiMan;
    MultiplierManager multipliersMan;
    GamePlayManager gamePlayMan;
    public GuessResponse guessResponse;
    public bool IsGuessDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
        multipliersMan = CommandCenter.Instance.multiplierManager_;
        gamePlayMan = CommandCenter.Instance.gamePlayManager_;
    }

    [ContextMenu("Guess")]
    public void Guess ()
    {
        IsGuessDone = false;
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        bool IsFirstTime = gamePlayMan.Get_IsFirstTime();
        bool IsFromSkip = gamePlayMan.GetIsFromSkipping();

        //if is firstTime or if is not first time && if skip or not
        GameState selectedGameState;
        string selectedSignature;

        switch (IsFirstTime)
        {
            case true:
                selectedGameState = apiMan.StartApi.gameResponse.game_state;
                selectedSignature = apiMan.StartApi.gameResponse.signature;
                //Debug.Log("Using StartApi game_state & signature");
                break;
            case false:
                switch (IsFromSkip)
                {
                    case true:
                        selectedGameState = apiMan.SkipApi.skipResponse.game_state;
                        selectedSignature = apiMan.SkipApi.skipResponse.signature;
                       // Debug.Log("Using guessResponse game_state & signature");
                        break;
                    case false:
                        selectedGameState = guessResponse.game_state;
                        selectedSignature = guessResponse.signature;
                        //Debug.Log("Using guessResponse game_state & signature");
                        break;
                }
                break;
        }

        Debug.Log($"Selected GameState: {selectedGameState}");
        Debug.Log($"Selected Signature: {selectedSignature}");

        GuessRequest request = new GuessRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.SetBetId() ,
            game_state = selectedGameState ,
            bet_choice = multipliersMan.selectedMultiplier.ToString() ,
            signature = selectedSignature ,
        };

        string jsonData = JsonConvert.SerializeObject(request , settings);
        Debug.Log($"Guess api request:{jsonData}");
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
                IsGuessDone = true;
                PromptManager.Instance.ShowErrorPrompt(webRequest.result.ToString() , webRequest.error);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                guessResponse = JsonConvert.DeserializeObject<GuessResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"Guess api response:{formattedOutput}");
                IsGuessDone=true;
            }
        }
    }
}
