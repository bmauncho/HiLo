using UnityEngine;
using System.Globalization;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Config_Assets
{

    public class BetUiMan : MonoBehaviour
    {
        [Header("Usable")]
        public float BetAmount;
        public float BetDenomination;
        public float BetMultipliers;
        public UnityEvent OnBetRefresh;
        [Header("Assign")]
        public Button Next_Btn;
        public Button Prev_Btn;
        [Header("Stuff")]
        public Animator Anim;
        public GameObject BtnPref;
        public Transform SpawnPos;
        BetUi active;
        public TMP_Text BetAmountText;
        public TMP_Text WinAmountText;
        public TMP_Text BalanceAmountText;
        bool init;
        private void OnEnable()
        {
            Setup();
        }
        public void Setup()
        {
            if (init)
                return;
            if (!ConfigMan.Instance)
            {
                Invoke(nameof(Setup), 0.1f);
            }
            else
            {
                Spawn();
            }
        }
        [ContextMenu("Spawn")]
        public void Spawn()
        {
            init = true;
            GameObject defaultobj = null;
            Clear();
            for(int i = 0; i < ConfigMan.Instance.BetValues.Length; i++)
            {
                float betvalue = float.Parse(ConfigMan.Instance.BetValues[i],CultureInfo.InvariantCulture);
                float betDenomination = 0;
                float betBetMultipliers = 0;
                if (ConfigMan.Instance.BetDenomiations.Length > i)
                {
                    betDenomination = float.Parse(ConfigMan.Instance.BetDenomiations[i], CultureInfo.InvariantCulture);
                }
                if (ConfigMan.Instance.BetMultipliers.Length > i)
                {
                    betBetMultipliers = float.Parse(ConfigMan.Instance.BetMultipliers[i], CultureInfo.InvariantCulture);
                }
                GameObject go = Instantiate(BtnPref, SpawnPos);
                go.GetComponent<BetUi>().betUiMan = this;
                go.GetComponent<BetUi>().Setup(betvalue, betDenomination, betBetMultipliers);
                if (defaultobj == null)
                {
                    defaultobj = go;
                }

            }
            if (defaultobj)
            {
                UpdateActive(defaultobj.GetComponent<BetUi>());
            }
        }
        public void UpdateWalletBalance(float Amount)
        {
            BalanceAmountText.text = Amount.ToString("n2");
        }
        public void UpdateWinAmount(float Amount)
        {
            WinAmountText.text = Amount.ToString("n2");
        }
        public void UpdateActive(BetUi betUi)
        {
            active = betUi;
            BetAmount = betUi.betvalue;
            BetDenomination = betUi.betDenomination;
            BetMultipliers = betUi.betMultipliers;
            BetAmountText.text = BetAmount.ToString();
            RefreshAll();
            OnBetRefresh.Invoke();

        }
        void RefreshAll()
        {
            BetUi[] temp = SpawnPos.GetComponentsInChildren<BetUi>(true);
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == active)
                {
                    temp[i].Refresh(true);
                }
                else
                {
                    temp[i].Refresh(false);
                }
            }
            int current = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == active)
                {
                    current = i;
                    break;
                }
            }
            RefreshBtns(current, temp.Length);

        }
        void Clear()
        {
            BetUi[] temp = SpawnPos.GetComponentsInChildren<BetUi>(true);
            for(int i = 0; i < temp.Length; i++)
            {
                Destroy(temp[i].gameObject);
            }
        }
        [ContextMenu("Open")]
        public void Open()
        {
            gameObject.SetActive(true);
            Anim.Play("Open");
        }
        public void Close()
        {
            gameObject.SetActive(false);

        }

        [ContextMenu("Close")]
        public void DelayClose()
        {
            Invoke(nameof(Close), 0.5f);
            Anim.Play("Close");
        }
        [ContextMenu("Bet_Add")]
        public void Bet_Add()
        {
            int current = 0;
            BetUi[] temp = SpawnPos.GetComponentsInChildren<BetUi>(true);
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == active)
                {
                    current = i;
                    break;
                }
            }
            current += 1;
            if (current > temp.Length - 1)
            {
                current = temp.Length - 1;
            }
            temp[current].Pressed();

        }
        [ContextMenu("Bet_Minus")]
        public void Bet_Minus()
        {
            int current = 0;
            BetUi[] temp = SpawnPos.GetComponentsInChildren<BetUi>(true);
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == active)
                {
                    current = i;
                    break;
                }
            }
            current -= 1;
            if (current <0)
            {
                current = 0;
            }
            temp[current].Pressed();
        }
        void RefreshBtns(int current,int maxlenght)
        {
            if (!Next_Btn || !Prev_Btn)
                return;
            if (current <= 0)
            {
                Prev_Btn.interactable = false;
                Next_Btn.interactable = true;
            }
            else if (current >=maxlenght-1)
            {
                Prev_Btn.interactable = true;
                Next_Btn.interactable = false;
            }
            else
            {
                Prev_Btn.interactable = true;
                Next_Btn.interactable = true;
            }
        }
    }
}
