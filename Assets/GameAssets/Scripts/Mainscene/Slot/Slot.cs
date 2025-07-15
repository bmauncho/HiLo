using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject TheOwner;
   
    public void SetOwner ( GameObject owner )
    {
        if(TheOwner != null)
        {
            Debug.LogWarning("Slot already has an owner. Removing previous owner.");
            return;
        }
        TheOwner = owner;
    }

    public void RemoveOwner ()
    {
        if (TheOwner != null)
        {
            TheOwner = null;
        }
    }
}
