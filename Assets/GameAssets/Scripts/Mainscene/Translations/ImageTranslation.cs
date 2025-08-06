using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ImageTranslationInfo
{
    public TheLanguage language;
    public Sprite Translation;
}
public class ImageTranslation : MonoBehaviour
{
    [HideInInspector]public Image Image;
    public TheLanguage language;
    public ImageTranslationInfo [] translationInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Image = GetComponent<Image>();
        if (LanguageMan.instance != null)
        {
            LanguageMan.instance.onLanguageRefresh.AddListener(RefreshLanguage);
            if (HasThelanguageChanged())
            {
                RefreshLanguage();
                language = LanguageMan.instance.ActiveLanguage;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshLanguage ()
    {
        Sprite fallback = null;

        foreach (var translation in translationInfo)
        {
            // Store the English sprite as fallback
            if (translation.language == TheLanguage.English)
            {
                fallback = translation.Translation;
            }

            // If there's a match with the current language, use it
            if (translation.language == LanguageMan.instance.ActiveLanguage)
            {
                Image.sprite = translation.Translation;
                return;
            }
        }

        // Fallback to English if no match was found
        if (fallback != null)
        {
            Image.sprite = fallback;
        }
        else
        {
            Debug.LogWarning("No translation found for current language or English fallback.");
        }
    }

    public bool HasThelanguageChanged ()
    {
        return language != LanguageMan.instance.ActiveLanguage;
    }

}
