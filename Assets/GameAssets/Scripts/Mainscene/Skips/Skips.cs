using DG.Tweening;
using UnityEngine;

public class Skips : MonoBehaviour
{
    public RectTransform skipsHolder;
    public GameObject [] availableSkips;
    [SerializeField] private bool isFirstTime = true;
    [SerializeField] private bool canSkip = true;
    [SerializeField] private bool isGamePlaySkipsActive = false;
    [SerializeField] private int currentSkipIndex;
    private Tween showSkips;
    private Tween hideSkips;
    private void OnEnable ()
    {
        currentSkipIndex = availableSkips.Length;
        ResetSkips();
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
        if(isFirstTime)
        {
            currentSkipIndex = 0;

            foreach (GameObject skipIcon in availableSkips)
            {
                skipIcon.SetActive(false);
            }
        }
        else
        {
            currentSkipIndex = availableSkips.Length;

            foreach (GameObject skipIcon in availableSkips)
            {
                skipIcon.SetActive(true);
            }
        } 
    }

    public void ActivateGameplaySpins ()
    {
        hideSkips.Kill();
        skipsHolder.gameObject.SetActive(true);
        showSkips = skipsHolder.DOAnchorPosY(-13f , .25f);
        isGamePlaySkipsActive = true;
    }

    public void DeactivateGameplaySpins ()
    {
        showSkips.Kill();
        skipsHolder.gameObject.SetActive(false);
        hideSkips = skipsHolder.DOAnchorPosY(0f , .25f);
        isGamePlaySkipsActive = false;
    }

    public bool AllowSkip ()
    {
        if (isGamePlaySkipsActive)
        {
            if(currentSkipIndex > 0)
            {
                return true;
            }
            else
            {
                return false;
            }  
        }
        else
        {

            return true;

        }
    }

    public void setIsFirstTime(bool value )
    {
        isFirstTime = value;
    }

    public bool IsFirstTime ()
    {
        return isFirstTime;
    }
}
