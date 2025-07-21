using TMPro;
using UnityEngine;

public class Multiplier : MonoBehaviour
{
    public MultiplierType multiplier;
    public TMP_Text multiplierText;
    public GameObject Mask;

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
}
