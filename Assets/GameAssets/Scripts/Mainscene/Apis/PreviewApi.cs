using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
[System.Serializable]
public class PreviewRequest
{
    public string client_id;
    public string game_id;
    public string player_id;
    public string bet_id;
    public float bet_amount;
}

[System.Serializable]
public class PreviewResponse
{
    public string status;
    public string message;
    public string current_card;
    public BetOptions [] bet_options;
}

public class PreviewApi : MonoBehaviour
{
    public PreviewResponse response;
    public bool IsPreviewDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void InitializeGame ()
    {
        IsPreviewDone = false;
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        PreviewRequest request = new PreviewRequest
        {
            client_id = GameManager.Instance.GetClientId() ,
            game_id = GameManager.Instance.GetGameId() ,
            player_id = GameManager.Instance.GetPlayerId() ,
            bet_id = ConfigMan.Instance.GetBetId() ,
            bet_amount = 5 ,
        };

        string jsonData = JsonConvert.SerializeObject(request , settings);
        Debug.Log($"Initialize api request:{jsonData}");
        StartCoroutine(GuessAction(jsonData));
    }

    IEnumerator GuessAction ( string jsonData )
    {
        string baseUrl = ConfigMan.Instance.Base_url;
        string ApiUrl = "https://b.api.ibibe.africa" + "/preview/hilo";
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
                IsPreviewDone = true;
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                response = JsonConvert.DeserializeObject<PreviewResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"Intialize api response:{formattedOutput}");
                IsPreviewDone = true;
            }
        }
    }
}
