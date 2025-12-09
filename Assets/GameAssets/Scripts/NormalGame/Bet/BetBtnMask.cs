using UnityEngine;
using UnityEngine.UI;

public class BetBtnMask : MonoBehaviour
{
    public GameObject Mask;
   //[SerializeField]private bool isActive = false;
    public void ActivateMask ()
    {
        GetComponent<Button>().interactable = false;
        Mask.SetActive (true);
    }

    public void DeactivateMask ()
    {
        GetComponent<Button>().interactable = true;
        Mask .SetActive (false);
    }
}
