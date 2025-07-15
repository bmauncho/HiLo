using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Refrences")]
    public Settings settings_;
    public GameRules gameRules_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSettings ()
    {
        settings_.ToggleSettings();
    }

    public void ToggleHelp ()
    {
        gameRules_.ToggleGameRules();
    }

    public void ToggleGamePlay ()
    {
        CommandCenter.Instance.gamePlayManager_.ToggleGamePlay();
    }
}
