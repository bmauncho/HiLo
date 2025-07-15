using UnityEngine;

public class Skips : MonoBehaviour
{
    public GameObject skipsHolder;
    public GameObject [] availableSkips;

    [SerializeField] private bool canSkip = true;
    [SerializeField] private bool isGamePlaySkipsActive = false;
    [SerializeField] private int currentSkipIndex;

    private void OnEnable ()
    {
        currentSkipIndex = availableSkips.Length;
        ResetSkips();
    }

    public void SetSkip ( bool canSkip_ = true)
    {
        canSkip = canSkip_;
    }

    public void SkipCard ()
    {
        if (canSkip)
        {
            if(isGamePlaySkipsActive && currentSkipIndex > 0)
            {
                Debug.Log("Gameplay skip : Card skipped.");
                currentSkipIndex--;
                availableSkips [currentSkipIndex].SetActive(false);
            }
            else
            {
                // TODO: Add logic to skip the card here.
                Debug.Log("Normal skip : Card skipped.");
            }
        }
    }

    public void ResetSkips ()
    {
        currentSkipIndex = availableSkips.Length;

        foreach (GameObject skipIcon in availableSkips)
        {
            skipIcon.SetActive(true);
        }
    }

    public void ActivateGameplaySpins ()
    {
        skipsHolder.SetActive(true);
        isGamePlaySkipsActive = true;
    }

    public void DeactivateGameplaySpins ()
    {
        skipsHolder.SetActive(false);
        isGamePlaySkipsActive = false;
    }
}
