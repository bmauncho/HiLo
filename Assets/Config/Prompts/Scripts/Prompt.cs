using UnityEngine;
using TMPro;
public class Prompt : MonoBehaviour
{
    public PromptType TheType;
    public TMP_Text CodeErrorText;
    public TMP_Text DetailedErrorText;
    public void ShowDetailedError(string _s)
    {
        DetailedErrorText.text = _s;
    }
    public void ShowErrorCode(string _s)
    {
        CodeErrorText.text = _s;
    }
}
