using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PayOut : MonoBehaviour
{
    WinLoseManager winLoseManager;
    public GameObject Holder;
    public GameObject EffectHolder;
    public TMP_Text cashOut;
    public TMP_Text amountWon;
    public Action PayoutEffectComplete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winLoseManager = CommandCenter.Instance.winLoseManager_;
    }

    public IEnumerator ShowPayOut ()
    {
        if(winLoseManager.GetTheOutCome() == OutCome.Lose || 
            winLoseManager.GetTheOutCome() == OutCome.None)
        {
            Debug.Log("Lost or None");
            PayoutEffectComplete?.Invoke();
            yield break;
        }
        EnableHolder();
        EnableEffectHolder();
        Holder.transform.DOPunchScale(new Vector3(0.2f , 0.2f , 0.2f) , 1f , 0 , 1);
        SetCashOut_Amount();
        SetAmountWon_Amount();
        yield return new WaitForSeconds(3);
        PayoutEffectComplete?.Invoke();
        Debug.Log("Payout Done!");
        DisableHolder();
        DisableEffectHolder();
        yield return null;
    }

    public void EnableHolder ()
    {
        Holder.SetActive(true);
    }
    
    public void DisableHolder ()
    {
        Holder.SetActive(false);
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
        cashOut.text = Amount;
        cashOut.GetComponent<TextHelper>().ManualRefresh(Amount);
    }

    public void SetAmountWon_Amount (string Amount = "1.00")
    {
        amountWon.text = Amount;
        amountWon.GetComponent<TextHelper>().ManualRefresh(Amount);
    }
}
