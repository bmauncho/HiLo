using DG.Tweening;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private bool isSettingsOpen = false;
    public RectTransform settingsUI;

    public void ToggleSettings ()
    {
        if (isSettingsOpen)
        {
            closeSettings();
        }
        else
        {
            openSettings();
        }
    }

    public void openSettings ()
    {
        isSettingsOpen = true;
        settingsUI.DOAnchorPosY(46f , .25f);
    }

    public void closeSettings ()
    {
        isSettingsOpen = false;
        settingsUI.DOAnchorPosY(160f , .25f);
    }
}
