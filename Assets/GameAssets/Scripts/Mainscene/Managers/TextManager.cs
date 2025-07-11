using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public TMP_SpriteAsset mainText;
    public TMP_SpriteAsset coloredText;

    public List<(string actual, string available)> charRefrences = new List<(string actual, string available)>
    {
        (".","fullstop"),
        (",", "comma")
    };

    #region[example]

    [HideInInspector] public TMP_Text testText;
    [HideInInspector] public string Input;
    private void Test ()
    {
        TextGenerator.CreateSpriteAssetText(
            Input ,
            GetColoredTextRef(),
            GetUpperCaseTextRef() ,
            testText,
            GetColoredFont(),
            charRefrences);
    }
    #endregion

    public TMP_SpriteAsset GetMainFont ()
    {
        return mainText;
    }

    public TMP_SpriteAsset GetColoredFont ()
    {
        return coloredText;
    }

    public string GetColoredTextRef ()
    {
        return "Font_Colored_";
    }

    public string GetMainTextRef ()
    {
        return "Font_main_";
    }

    public string GetUpperCaseTextRef ()
    {
        return "Uc_";
    }

    public List<(string actual, string available)> GetCharacterReferences ()
    {
        return charRefrences;
    }
}
