using UnityEngine;
using TMPro;
namespace Config_Assets
{
    public class BetUi : MonoBehaviour
    {
        public GameObject ActiveObj;
        public TMP_Text TheText;
        public float betvalue;
        public float betDenomination;
        public float betMultipliers;
        public BetUiMan betUiMan;
        public void Refresh(bool IsActive)
        {
            ActiveObj.SetActive(IsActive);
        }
        public void Pressed()
        {
            betUiMan.UpdateActive(this);
           betUiMan.DelayClose();
        }
        public void Setup(float _betvalue,float _denomination,float _multiplier) 
        {
            betvalue = _betvalue;
            betDenomination = _denomination;
            betMultipliers = _multiplier;
            TheText.text = betvalue.ToString();
        }
    }
}
