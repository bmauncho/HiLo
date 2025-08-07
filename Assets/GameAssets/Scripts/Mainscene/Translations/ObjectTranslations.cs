using UnityEngine;

[System.Serializable]
public class ObjectTranslationInfo
{
    public TheLanguage language;
    public GameObject Translation;
}

public class ObjectTranslations : MonoBehaviour
{
    public TheLanguage language;
    public ObjectTranslationInfo [] translationInfo;

    void Start ()
    {
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

    public void RefreshLanguage ()
    {
        GameObject fallback = null;

        // First disable all translations
        foreach (var info in translationInfo)
        {
            if (info.Translation != null)
                info.Translation.SetActive(false);
        }

        // Then find the right one to activate
        foreach (var info in translationInfo)
        {
            if (info.language == TheLanguage.English)
                fallback = info.Translation;

            if (info.language == LanguageMan.instance.ActiveLanguage)
            {
                if (info.Translation != null)
                    info.Translation.SetActive(true);
                return;
            }
        }

        // Fallback to English
        if (fallback != null)
        {
            fallback.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No translation found for current language or fallback.");
        }
    }

    public bool HasThelanguageChanged ()
    {
        return language != LanguageMan.instance.ActiveLanguage;
    }
}
