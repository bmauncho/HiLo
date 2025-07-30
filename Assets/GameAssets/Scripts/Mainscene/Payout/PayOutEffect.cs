using UnityEngine;

public class PayOutEffect : MonoBehaviour
{
    public bool CanSpin = false;
    public float degreesPerSecond = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update ()
    {
        if (CanSpin)
        {
            transform.Rotate(new Vector3(0 , 0 , degreesPerSecond) * Time.deltaTime);
        }
    }

    public void startSpin ()
    {
        CanSpin = true;
    }

    public void stopSpinning ()
    {
        CanSpin = false;
    }
}
