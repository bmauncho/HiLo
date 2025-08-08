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
    public double final_win;
    public GameState game_state;
}
public class CashOutApi : MonoBehaviour
{
    ApiManager apiMan;
    GamePlayManager gamePlayMan;
    public CashOutResponse cashOutResponse;
    public bool IsCashOutDone = false;
    public bool init;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
        gamePlayMan =  CommandCenter.Instance.gamePlayManager_;
    }

    public void CashOut ()
    {
        IsCashOutDone = false;

        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        bool IsFirstTime = gamePlayMan.Get_IsFirstTime();
        bool IsSkip = gamePlayMan.Get_IsSkip();

        //if is firstTime or if is not first time && if skip or not
        GameState selectedGameState;
        string selectedSignature;

        switch (IsSkip)
        {
            case true:
                selectedGameState = apiMan.SkipApi.skipResponse.game_state;
                selectedSignature = apiMan.SkipApi.skipResponse.signature;
                //Debug.Log("Using SkipApi game_state & signature");
                break;
            case false:
                switch (apiMan.IsFirstPlayDone)
                {
                    case true:

                        selectedGameState = apiMan.guessApi.guessResponse.game_state;
                        selectedSignature = apiMan.guessApi.guessResponse.signature;
                        //Debug.Log("Using guessResponse game_state & signature");
                        break;
                    case false:
                        selectedGameState = apiMan.StartApi.gameResponse.game_state;
                        selectedSignature = apiMan.StartApi.gameResponse.signature;
                        //Debug.Log("Using StartApi game_state & signature");
                        init = true;
                        break;
                }

                break;
        }

        Debug.Log($"Selected GameState: {selectedGameState}");
        Debug.Log($"Selected Signature: {selectedSignature}");

        CashOutRequest request = new CashOutRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.GetBetId() ,
            game_state = selectedGameState,
            signature = selectedSignature,
        };

        string jsonData = JsonConvert.SerializeObject(request , settings);
        Debug.Log($"cashOut api request:{jsonData}");
        StartCoroutine(CashOutAction(jsonData));
    }

    IEnumerator CashOutAction (string jsonData)
    {
        string testUrl = "https://b.api.ibibe.africa";
        string baseUrl = ConfigMan.Instance.Base_url;
        string ApiUrl =  testUrl + "/cashout/hilo";
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
                IsCashOutDone = true;
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                cashOutResponse = JsonConvert.DeserializeObject<CashOutResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"cashout api response:{formattedOutput}");
                IsCashOutDone = true ;
            }
        }
    }
}
