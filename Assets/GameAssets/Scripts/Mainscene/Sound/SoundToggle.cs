using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    Toggle soundToggle;
    [SerializeField] private bool TheSound;
    private Image toggle;
    [SerializeField] private Sprite on_;
    [SerializeField] private Sprite off_;

    private void Start ()
    {
        soundToggle = GetComponent<Toggle>();
        toggle = GetComponent<Image>();
        Refresh();
    }

    public void SoundPressed ()
    {
        CommandCenter.Instance.settingsManager_.ToogleSound(soundToggle.isOn);
        TheSound = CommandCenter.Instance.settingsManager_.Sound;
        if (soundToggle.isOn)
        {
            toggle.sprite = on_;
        }
        else
        {
            toggle.sprite = off_;
        }
        DisableSettings();
    }

    public void Refresh ()
    {
        TheSound = CommandCenter.Instance.settingsManager_.Sound;
        soundToggle.isOn = TheSound;
        if (soundToggle.isOn)
        {
            toggle.sprite = on_;
        }
        else
        {
            toggle.sprite = off_;
        }
    }

    void DisableSettings ()
    {
        //CommandCenter.Instance.mainMenuController_.ToggleSettings();
    }
}
