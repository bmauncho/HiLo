using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class PreviewSkipRequest
{
    public string client_id;
    public string game_id;
    public string player_id;
    public string bet_id;
    public float bet_amount;
    public string current_card;
}

[System.Serializable]
public class PreviewSkipResponse
{
    public string status;
    public string message;
    public string current_card;
    public BetOptions [] bet_options;
}
public class PreviewSkipApi : MonoBehaviour
{
    ApiManager apiManager;
    public bool IsPreviewSkipDone = false;
    public PreviewSkipResponse response;
    bool Init = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiManager = CommandCenter.Instance.apiManager_;
    }

    public void previewSkip ()
    {
        IsPreviewSkipDone = false;
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        bool isBetAmount = float.TryParse(apiManager.GetBetAmountValue() , out float betamount);

        bool IsFromGamePlay = CommandCenter.Instance.gamePlayManager_.GetIsFromGameplay();

        string currenCard = string.Empty;

        switch (IsFromGamePlay)
        {
            case true:
                currenCard = apiManager.guessApi.guessResponse.game_state.current_card;
                CommandCenter.Instance.gamePlayManager_.SetIsFromGamePlay(false);
                break;
            case false:
                switch (Init)
                {
                    case true:
                        currenCard = GameManager.Instance.previewApi.response.current_card;
                        Init = false;
                        break;
                    case false:
                        currenCard = response.current_card;
                        break;
                }
               
                break;
        }

        PreviewSkipRequest request = new PreviewSkipRequest
        {
            client_id=apiManager.GetClientId(),
            game_id=apiManager.GetGameId(),
            player_id=apiManager.GetPlayerId(),
            bet_id=apiManager.SetBetId(),
            bet_amount= isBetAmount?betamount:5,
            current_card = currenCard,
        };

        string jsonData = JsonConvert.SerializeObject(request , settings);
        Debug.Log($"preview-skip api request:{jsonData}");
        StartCoroutine(skip(jsonData));
    }

    IEnumerator skip(string jsonData )
    {
        string testUrl = "https://b.api.ibibe.africa";
        string baseUrl = ConfigMan.Instance.Base_url;
        string ApiUrl = baseUrl + "/preview-skip/hilo";
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
                IsPreviewSkipDone = true;
                PromptManager.Instance.ShowErrorPrompt(webRequest.result.ToString() , webRequest.error);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                response = JsonConvert.DeserializeObject<PreviewSkipResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"preview-skip api response:{formattedOutput}");
                IsPreviewSkipDone = true;
            }
        }
    }
}
