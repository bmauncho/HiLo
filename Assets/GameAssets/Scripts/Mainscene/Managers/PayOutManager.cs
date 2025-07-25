using UnityEngine;

public class PayOutManager : MonoBehaviour
{
    public PayOut payout;
    public CashOutUI CashOutUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void ShowPayOut ()
    {
       StartCoroutine(payout.ShowPayOut ());
    }
}
