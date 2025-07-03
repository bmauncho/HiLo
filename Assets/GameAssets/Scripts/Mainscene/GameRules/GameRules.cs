using UnityEngine;

public class GameRules : MonoBehaviour
{
    [SerializeField] private bool IsGameRulesActive = false;
    [SerializeField] private GameObject [] PotraitItems;
    [SerializeField] private GameObject [] LandScapeItems;

    public enum Orientation { Portrait, Landscape }
    private Orientation previousOrientation;
    private Orientation CurrentOrientation
    {
        get
        {
            return Screen.width > Screen.height ? Orientation.Landscape : Orientation.Portrait;
        }
    }

    private void OnEnable ()
    {

    }

    private void Start ()
    {
        Debug.Log("Current Orientation: " + CurrentOrientation);
        this.gameObject.SetActive(false);
        Invoke(nameof(refreshUi) , 0.01f);
    }

    public void refreshUi ()
    {
        Invoke(nameof(EnableUI) , 0.025f);
        Invoke(nameof(DisabaleUi) , 0.05f);
        Invoke(nameof(EnableUI) , 0.075f);
    }

    void DisabaleUi ()
    {
        this.gameObject.SetActive(false);
    }

    void EnableUI ()
    {
        this.gameObject.SetActive(true);
    }
    [ContextMenu("Refresh LayOutItem")]
    public void RefreshLayOutItems ()
    {
        if (CurrentOrientation == Orientation.Portrait)
        {
            foreach (GameObject item in LandScapeItems)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in PotraitItems)
            {
                item.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject item in PotraitItems)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in LandScapeItems)
            {
                item.SetActive(true);
            }
        }
    }
    private void ShowGameRules ()
    {
        IsGameRulesActive = true;
        gameObject.SetActive(true);
    }

    private void HideGameRules ()
    {
        IsGameRulesActive = false;
        gameObject.SetActive(false);
    }

    public void ToggleGameRules ()
    {
        if (IsGameRulesActive)
        {
            HideGameRules();
        }
        else
        {
            ShowGameRules();
        }
    }
    void Update ()
    {
        Orientation current = Screen.width > Screen.height ? Orientation.Landscape : Orientation.Portrait;

        if (current != previousOrientation)
        {
            previousOrientation = current;
            OnOrientationChanged(current);
            Canvas.ForceUpdateCanvases(); // Force update the canvas to apply layout changes immediately
        }
    }

    void OnOrientationChanged ( Orientation newOrientation )
    {
        Debug.Log("Orientation changed to: " + newOrientation);
        RefreshLayOutItems();
    }
}
