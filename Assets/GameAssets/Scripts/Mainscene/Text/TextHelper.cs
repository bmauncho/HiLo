using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextHelper : MonoBehaviour
{
    private TextManager textManager;
    private TMP_Text textComponent;
    private string lastProcessedText = "";

    public bool IsColored = false;
    public bool IsManual = false;

    private void Start ()
    {
        if (CommandCenter.Instance != null)
        {
            textManager = CommandCenter.Instance.textManager_;
        }
        else
        {
            Debug.LogWarning("CommandCenter.Instance is null in TextHelper");
        }
        textComponent = GetComponent<TMP_Text>();
        if (IsManual) { return; }
        RefreshIfChanged();
    }
  
    // Public method to call when the text might have changed
    public void RefreshIfChanged ()
    {
        if (!CommandCenter.Instance)
        {
            return;
        }

        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }

        if (textComponent == null)
            return;

        string currentText = textComponent.text;

        if(currentText == lastProcessedText)
        {
            return;
        }

        if (currentText != lastProcessedText)
        {
            lastProcessedText = currentText;
            Refresh();
        }
    }

    public void Refresh ()
    {
        //refrences
        TMP_Text text = textComponent;
        string Input = text.text;
        string prefix = IsColored ? textManager.GetColoredTextRef() : textManager.GetMainTextRef();
        string UpperCase = textManager.GetUpperCaseTextRef();
        TMP_SpriteAsset spriteAsset = IsColored ? textManager.GetColoredFont() : textManager.GetMainFont();
        List<(string actual, string available)> charRefrences = new List<(string actual, string available)>(textManager.GetCharacterReferences());

       // Debug.Log(Input);
        //call
        TextGenerator.CreateSpriteAssetText(
            Input ,
            prefix ,
            UpperCase ,
            text ,
            spriteAsset ,
            charRefrences);
    }

    public void ManualRefresh(string input )
    {
        TextManager textManager = CommandCenter.Instance.textManager_;
        TMP_Text text = GetComponent<TMP_Text>();
        string Input = input;
        string prefix = IsColored ? textManager.GetColoredTextRef() : textManager.GetMainTextRef();
        string UpperCase = textManager.GetUpperCaseTextRef();
        TMP_SpriteAsset spriteAsset = IsColored ? textManager.GetColoredFont() : textManager.GetMainFont();
        List<(string actual, string available)> charRefrences = new List<(string actual, string available)>(textManager.GetCharacterReferences());

        //Debug.Log(Input);
        //call
        TextGenerator.CreateSpriteAssetText(
            Input ,
            prefix ,
            UpperCase ,
            text ,
            spriteAsset ,
            charRefrences);
    }

    [ContextMenu("Refresh"),Tooltip("Use only when in play mode !")]
    void manualOverride ()
    {
        TextManager textManager = CommandCenter.Instance.textManager_;
        TMP_Text text = GetComponent<TMP_Text>();
        string Input = text.text;
        string prefix = IsColored ? textManager.GetColoredTextRef() : textManager.GetMainTextRef();
        string UpperCase = textManager.GetUpperCaseTextRef();
        TMP_SpriteAsset spriteAsset = IsColored ? textManager.GetColoredFont() : textManager.GetMainFont();
        List<(string actual, string available)> charRefrences = new List<(string actual, string available)>(textManager.GetCharacterReferences());

        Debug.Log(Input);
        //call
        TextGenerator.CreateSpriteAssetText(
            Input ,
            prefix ,
            UpperCase ,
            text ,
            spriteAsset ,
            charRefrences);
    }
}
