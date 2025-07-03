using System.Collections;
using TMPro;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance;
    [SerializeField] private GameObject Prompt;
    [SerializeField] private GameObject BrokenImgIcon;
    [SerializeField] private GameObject BrokenText;
    [SerializeField] private TMP_Text error;
    [SerializeField] private TMP_Text errorCode;
    [SerializeField] private TMP_Text errorDescription;
    [SerializeField] private TMP_Text errorRefNo;
    [SerializeField] private TMP_Text btnText;
    void Awake ()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }
    [ContextMenu("Show Prompt")]
    public void ShowPrompt ()
    {
        Prompt.SetActive(true);
    }
    [ContextMenu("Hide Prompt")]
    public void HidePrompt ()
    {
        BrokenImgIcon.SetActive(true);
        BrokenText.SetActive(false);
        Prompt.SetActive(false);
    }

    public void ShowErrorPrompt ( 
        string errorCode_ ,
        string errorDescription_,
        string errorRefNo_ = "1750137774545@cb735343-6039-4efb-80f5-3f3f6dda3fe9" ,
        string btnText_ = "OK" )
    {
        ShowPrompt();
        StartCoroutine(errorVisualizer(
            errorCode_,
            errorDescription_ ,
            errorRefNo_ ,
            btnText_
        ));
    }

    IEnumerator errorVisualizer ( 
        string errorCode_ , 
        string errorDescription_ , 
        string errorRefNo_ = "1750137774545@cb735343-6039-4efb-80f5-3f3f6dda3fe9" , 
        string btnText_ = "OK"  )
    {

        BrokenImgIcon.SetActive(true);
        BrokenText.SetActive(false);

        if (string.IsNullOrEmpty(errorCode_))
        {
            errorCode_ = "1";
        }
        else if (errorCode_ == "0")
        {
            errorCode_ = "1";
        }

        error.text = "Error";
        errorCode.text = "Code : " + errorCode_;
        errorDescription.text = errorDescription_;
        errorRefNo.text = errorRefNo_;
        btnText.text = btnText_;
        yield return new WaitForSeconds(5f);
        BrokenImgIcon.SetActive(false);
        BrokenText.SetActive(true);

        yield return null;
    }
}
