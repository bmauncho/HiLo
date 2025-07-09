using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public TMP_SpriteAsset mainText;
    public TMP_SpriteAsset coloredText;
    public TMP_Text testText;
    public string Input;
    public List<(string actual, string available)> charRefrences = new List<(string actual, string available)>
    {
        (".","fullstop"),
        (",", "comma")
    };

    [ContextMenu("Test")]
    public void Test ()
    {
        TextGenerator.CreateSpriteAssetText(
            Input ,
            "Font_main_" ,
            "Uc_" ,
            testText,
            mainText,
            charRefrences);
    }
}
