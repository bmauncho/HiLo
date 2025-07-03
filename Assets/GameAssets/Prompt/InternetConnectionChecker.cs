using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public enum InternetConnection
{
    Unavailable,
    Bad,
    Okay,
    Good,
    Best
}
public class InternetConnectionChecker : MonoBehaviour
{
    public bool IsInternetEnabled = false;
    private bool IsOffLine = false;
    private float checkInterval = 5f; // Time in seconds between checks
    private float timer = 0f;
    private int maxRetries = 3; // Number of retry attempts on failure
    private int retryCount = 0;
    private string errorCode;
    public InternetConnection connection;

    void Update ()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckInternetReachability();
        }
    }

    void CheckInternetReachability ()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            IsInternetEnabled = false;
            connection = InternetConnection.Unavailable;
            // Debug.Log("No internet reachability detected.");
            HandleNoInternetConnection();
        }
        else
        {
            //Debug.Log("Internet reachability detected. Proceeding to connection quality check.");
            StartCoroutine(CheckInternetConnectionCoroutine());
        }
    }

    private IEnumerator CheckInternetConnectionCoroutine ()
    {
        //Debug.Log("Starting connection quality test using Option A.");
        yield return StartCoroutine(CheckInternetQuality());

        if (!IsInternetEnabled)
        {
            connection = InternetConnection.Bad;
            //  Debug.Log("Connection deemed BAD after quality test.");
            HandleNoInternetConnection();
        }
    }

    IEnumerator CheckInternetQuality ()
    {
        string testUrl = "https://www.cloudflare.com/cdn-cgi/trace";

        using (UnityWebRequest www = UnityWebRequest.Get(testUrl))
        {
            www.timeout = 5; // Timeout for the request
            float startTime = Time.time;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                float responseTime = Time.time - startTime;

                IsInternetEnabled = true;
                retryCount = 0;

                // Determine connection quality based on response time
                if (responseTime < 0.5f)
                {
                    connection = InternetConnection.Best;
                }
                else if (responseTime < 1f)
                {
                    connection = InternetConnection.Good;
                }
                else
                {
                    connection = InternetConnection.Okay;
                }

                HandleInternetConnection();
            }
            else
            {
                retryCount++;
                IsInternetEnabled = false;

                if (retryCount >= maxRetries)
                {
                    connection = InternetConnection.Bad;
                    // Debug.Log("Max retries reached. Connection quality determined: BAD.");
                }
                else
                {
                    //  Debug.Log("Retrying connection...");
                }
                errorCode = www.responseCode.ToString();
            }
        }
    }

    void HandleNoInternetConnection ()
    {
        PromptManager.Instance.ShowErrorPrompt(
             errorCode,
            "Network Connection is Abnormal.Reconnecting..." 
        );
        IsOffLine = true;
    }

    void HandleInternetConnection ()
    {
        if (IsOffLine)
        {
            IsOffLine = false;
            PromptManager.Instance.HidePrompt();
        }
    }

    public bool IsInternetAvailable ()
    {
        bool isAvailable =
                IsInternetEnabled ||
                ( connection == InternetConnection.Best ) ||
                ( connection == InternetConnection.Good ) ||
                ( connection == InternetConnection.Okay ) ||
                ( connection == InternetConnection.Bad );
        return isAvailable;
    }
}
