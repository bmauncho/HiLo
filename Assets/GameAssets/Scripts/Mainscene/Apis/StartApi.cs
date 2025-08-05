using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class StartGameRequest
{
    public string client_id;
    public string game_id;
    public string player_id;
    public string bet_id;
    public float bet_amount;
    public string card;
}

[System.Serializable]
public class StartGameResponse
{
    public string status;
    public string message;
    public GameState game_state;
    public BetOptions [] bet_options;
    public string signature;
}

[System.Serializable]
public class GameState
{
    public string seed;
    public string deck_hash;
    public string current_card;
    public string unity_card;
    public int position;
    public float accumulated_win;
    public float bet_amount;
    public int skips_used;
    public int skips_remaining;
    public int max_skips;
    public GameHistory[] game_history;
    public bool is_game_over;
    public double final_win;
}

[System.Serializable]
public class BetOptions
{
    public string id;
    public string name;
    public float multiplier;
    public bool is_enabled;
}

[System.Serializable]
public class GameHistory
{
    public string card;
    public int value;
    public int position;
}


public class StartApi : MonoBehaviour
{
    ApiManager apiMan;
    CardManager cardMan;
    public StartGameResponse gameResponse;
    public bool IsStartDone =false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        apiMan = CommandCenter.Instance.apiManager_;
        cardMan = CommandCenter.Instance.cardManager_;
    }

    [ContextMenu("start Game")]
    public void startGame ()
    {
        IsStartDone = false;
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FloatTrimConverter());
        settings.Formatting = Formatting.Indented;

        CardData data = cardMan.GetCurrentCardData();
        
        StartGameRequest startGameRequest = new StartGameRequest
        {
            client_id = apiMan.GetClientId() ,
            game_id = apiMan.GetGameId() ,
            player_id = apiMan.GetPlayerId() ,
            bet_id = apiMan.SetBetId(),
            bet_amount = GetBetAmount(),
            card = $"{data.cardRank}_{data.cardSuite}",
        };

        string jsonData = JsonConvert.SerializeObject(startGameRequest , settings);
        Debug.Log($"Start api request:{jsonData}"); ;
        StartCoroutine(StartGame(jsonData));
    }

    IEnumerator StartGame (string jsonData)
    {
        string baseUrl = ConfigMan.Instance.Base_url;
        string ApiUrl = "https://b.api.ibibe.africa"+"/start/hilo";
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
                IsStartDone = true;
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                gameResponse = JsonConvert.DeserializeObject<StartGameResponse>(responseText);
                var parsedJson = JToken.Parse(responseText);
                string formattedOutput = JsonConvert.SerializeObject(parsedJson , Formatting.Indented);
                Debug.Log($"Start api response:{formattedOutput}");
                IsStartDone = true;
            }
        }
    }

    public float GetBetAmount ()
    {
        string betAmount = apiMan.GetBetAmountValue();
        float value;
        if (float.TryParse(betAmount , out  value))
        {
            Debug.Log("Parsed float: " + value);
        }
        else
        {
            Debug.LogWarning("Invalid float string: " + betAmount);
        }

        return value;
    }
}
