using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Multiplier : MonoBehaviour
{
    public MultiplierType multiplier;
    public TMP_Text multiplierText;
    public GameObject Mask;
    public Button guessBtn;

    private void Start ()
    {
        EnableMask();
        disableBtn();
    }

    public void SetText(string text )
    {
        multiplierText.text = text;
    }

    public void EnableMask ()
    {
        Mask.SetActive( true );
    }

    public void DisableMask ()
    {
        Mask.SetActive ( false );
    }

    public void enableBtn ()
    {
        guessBtn.interactable = true;
    }

    public void disableBtn ()
    {
        guessBtn.interactable = false;
    }

    public void Guess ()
    {
        CommandCenter.Instance.multiplierManager_.SetSelectedMultiplier( multiplier );
        CommandCenter.Instance.gamePlayManager_.Guess();
    }
}
