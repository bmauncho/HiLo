using TMPro;
using UnityEngine;

public class DemoText : MonoBehaviour
{
    public TMP_Text text;
    public string demoText = "demo000162";

    // Update is called once per frame
    void Update()
    {
        if (CommandCenter.Instance == null || !CommandCenter.Instance)
            return;

        if(CommandCenter.Instance.gameMode == GameMode.Demo)
        {
            text.gameObject.SetActive(true);
            text.text = demoText;
        }
        else
        {
            text.gameObject.SetActive(false);

        }
    }
}
