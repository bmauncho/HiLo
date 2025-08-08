using DG.Tweening;
using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class PayOut : MonoBehaviour
{
    WinLoseManager winLoseManager;
    PayOutManager payOutManager;
    public GameObject Holder;
    public GameObject EffectHolder;
    public GameObject Bg;
    public TMP_Text cashOut;
    public TMP_Text amountWon;
    public Action PayoutEffectComplete;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winLoseManager = CommandCenter.Instance.winLoseManager_;
        payOutManager = CommandCenter.Instance.PayOutManager_;
    }

    

    public IEnumerator ShowPayOut (PayOutManager payOutMan=null)
    {
        if(winLoseManager.GetTheOutCome() == OutCome.Lose || 
            winLoseManager.GetTheOutCome() == OutCome.None)
        {
            //Debug.Log("Lost or None");
            PayoutEffectComplete?.Invoke();
            yield break;
        }

        yield return StartCoroutine(payOutManager.updatePayout());


        string cashout_ = payOutMan.GetWinMultiplier();
        string amountwon_ = payOutMan.GetWinAmount().ToString("N2" , CultureInfo.CurrentCulture);

        Debug.Log($"cashout {cashout_} ; amountwon {amountwon_}");
        EnableHolder();
        EnableEffectHolder();
        UpdatePayoutUI(cashout_, amountwon_);
        payOutManager.CashOutUI.SetWinAmount(amountwon_);
        payOutManager.CashOutUI.UpdateWinAmount();
        CommandCenter.Instance.currencyMan_.updateCashOutWinings();
        CommandCenter.Instance.soundManager_.PlaySound("Win");
        Holder.transform.DOPunchScale(new Vector3(0.2f , 0.2f , 0.2f) , 1f , 0 , 1);
        yield return new WaitForSeconds(3);
        PayoutEffectComplete?.Invoke();
        Debug.Log("Payout Done!");
        DisableHolder();
        DisableEffectHolder();
        yield return null;
    }

    public void UpdatePayoutUI(string cashOutAmount, string winAmount )
    {   
        SetAmountWon_Amount(winAmount);
        SetCashOut_Amount(cashOutAmount);
    }

    public void EnableHolder ()
    {
        Holder.SetActive(true);
        Bg.SetActive(true);
    }
    
    public void DisableHolder ()
    {
        Holder.SetActive(false);
        Bg.SetActive(false);
    }

    public void EnableEffectHolder ()
    {
        EffectHolder.SetActive(true);
        PayOutEffect [] effects = GetComponentsInChildren<PayOutEffect>();
        foreach(var effect in effects)
        {
            effect.CanSpin = true;
        }
    }

    public void DisableEffectHolder ()
    {
        PayOutEffect [] effects = GetComponentsInChildren<PayOutEffect>();
        foreach (var effect in effects)
        {
            effect.CanSpin = false;
        }
        EffectHolder.SetActive(false);
    }

    public void SetCashOut_Amount (string Amount= "1.00")
    {
        cashOut.text = Amount + "x";
        cashOut.GetComponent<TextHelper>().ManualRefresh(Amount + "x");
    }

    public void SetAmountWon_Amount (string Amount = "1.00")
    {
        amountWon.text = Amount;
        amountWon.GetComponent<TextHelper>().ManualRefresh(Amount);
    }
}
