using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
public class LoadingScreen : MonoBehaviour
{
    [Header("Scene Reference")]
    [SerializeField] private AssetReference playScene;

    [Header("UI Elements")]
    [SerializeField] private bool isAddressablesEnabled = false;
    [SerializeField] private Image loadingSlider;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private GameObject LoadingContent;

    private float progress;
    private bool permissionAsked;
    private bool isSliderLoaded;
    private bool isSceneReady;
    private void Awake ()
    {
        
    }

    private void Start ()
    {
        isSliderLoaded = false;
        //isAddressablesEnabled = playScene == null ? false : true;
        StartCoroutine(LoadScene(playScene));
    }

    private void Update ()
    {
        //isAddressablesEnabled = playScene == null ? false : true;
        UpdateImageFillAmount(loadingSlider , progress);

        if (progress >= 1f && !permissionAsked)
        {
            permissionAsked = true;
        }

        if (progress >= 1f && permissionAsked)
        {
            if (!isSliderLoaded && !isAddressablesEnabled)
            {
                isSliderLoaded = true;
                // Show the start button or any other UI element
                if (startBtn)
                {
                    startBtn.SetActive(true);
                }

                if (LoadingContent)
                {
                    LoadingContent.SetActive(false);
                }
            }
        }
    }
    [ContextMenu("Activate")]
    public void Activate ()
    {
        if (isSceneReady) { return; }
        isSceneReady = true;
    }

    private void UpdateImageFillAmount ( Image image , float amount )
    {
        if (image != null)
            image.fillAmount = amount;
    }

    private void ShowProgress ( float amount )
    {
        progress = amount;
    }

    IEnumerator LoadScene ( AssetReference Which = null )
    {
        if (isAddressablesEnabled)
        {
            string loadingText = $"connecting...";
            setLoadingText(loadingText);
            float fakeprogress = Random.Range(0.05f , 0.3f);
            ShowProgress(fakeprogress);
            yield return new WaitForSeconds(0.5f);
            //Not allowing scene activation immediately
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(Which , LoadSceneMode.Additive , false);
            float fakeProgress = Random.Range(0.05f , 0.5f);
           
            while (!handle.IsDone)
            {
                ShowProgress(handle.PercentComplete);
                loadingText = $"downloading resources...[{( progress * 100f):F0}%]";
                yield return new WaitForSeconds(0.1f);
            }
            //One way to handle manual scene activation.

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ShowProgress(handle.PercentComplete);
                loadingText = $"downloading resources... [{( progress * 100f ):F0}%]";
                setLoadingText(loadingText);
            }

            yield return new WaitForSeconds(0.25f);
            loadingText = $"getting initiated...";
            setLoadingText(loadingText);
            yield return new WaitForSeconds(0.25f);
            loadingText = $"complete.";
            setLoadingText(loadingText);
            yield return new WaitForSeconds(0.5f);

            if (startBtn)
            {
                startBtn.SetActive(true);
            }

            if (LoadingContent)
            {
                LoadingContent.SetActive(false);
            }

            if (!startBtn && !LoadingContent)
            {
                Activate(); 
            }

            Debug.Log($"loading Addressable...Done ");
            yield return new WaitUntil(() => isSceneReady);

            Debug.Log($"isSceneReady: {isSceneReady}");

            GameManager.Instance.FetchConfigData();

            yield return new WaitUntil(() => GameManager.Instance.IsDataFetched());

            Debug.Log($"isDataFetched : {GameManager.Instance.IsDataFetched()}");
            handle.Result.ActivateAsync();
            ConfigMan.Instance.TheDebugObj.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            SceneManager.UnloadSceneAsync(0);
        }
        else
        {
            Debug.Log("loading Scene!");
            string loadingText = $"connecting...";
            setLoadingText(loadingText);
            AsyncOperation handle = SceneManager.LoadSceneAsync("MainScene" , LoadSceneMode.Additive);
            handle.allowSceneActivation = false;

            while (handle.progress < 0.9f)
            {
                ShowProgress(handle.progress);
                loadingText = $"downloading resources...[{( progress * 100f ):F0}%]";
                yield return new WaitForSeconds(0.1f);
            }

            if(handle.progress>= 0.9f && !isSceneReady)
            {
                float displayedProgress = handle.progress;
                // gradually increase the progress to 1
                while (displayedProgress<1)
                {
                    displayedProgress += 0.05f;
                    ShowProgress(displayedProgress);
                    loadingText = $"downloading resources...[{( progress * 100f ):F0}%]";
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(0.25f);
            loadingText = $"complete.";
            setLoadingText(loadingText);
            yield return new WaitForSeconds(0.5f);
            if (!startBtn && !LoadingContent)
            {
                Activate();
            }

            // Wait for your condition to be ready before activating scene
            yield return new WaitUntil(() => isSceneReady);
            GameManager.Instance.FetchConfigData();
            yield return new WaitUntil(() => GameManager.Instance.IsDataFetched());

            handle.allowSceneActivation = true;

            while (!handle.isDone)
            {
                yield return null;
            }

            ConfigMan.Instance.TheDebugObj.SetActive(false);
            //yield return new WaitForSeconds(0.5f);
            SceneManager.UnloadSceneAsync(0);
        }
    }

    public void setLoadingText (string loading_text)
    {
        if (loadingText != null)
        {
            loadingText.text = loading_text;
        }
        else
        {
            Debug.LogWarning("Loading Text is not assigned!");
        }
    }
}