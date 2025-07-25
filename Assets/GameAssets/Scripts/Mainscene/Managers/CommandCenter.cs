using UnityEngine;
[System.Serializable]
public enum GameMode
{
    Demo,
    Live,
}
public class CommandCenter : MonoBehaviour
{
    public static CommandCenter Instance { get; private set; }
    public GameMode gameMode; // Live or Demo
    public PoolManager poolManager_;
    public SettingsManager settingsManager_;
    public SoundManager soundManager_;
    public TextManager textManager_;
    public BetManager betManager_;
    public MainMenuManager mainMenuManager_;
    public CurrencyManager currencyMan_;
    public MultiplierManager multiplierManager_;
    public CardManager cardManager_;
    public GamePlayManager gamePlayManager_;
    public WinLoseManager winLoseManager_;
    public PayOutManager PayOutManager;
    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("This is extra!");
            //Destroy(gameObject); // Ensures only one instance exists
        }
        else
        {
            Instance = this;
        }

        if (GameManager.Instance)
        {
            SetUp();
            CheckifGameIsReady();
        }
    }

    void SetUp ()
    {
        gameMode = GameManager.Instance.IsDemo() ? GameMode.Demo : GameMode.Live;
        bool isDemo = GameManager.Instance.IsDemo() ? true : false;
    }

    void CheckifGameIsReady ()
    {
        Debug.Log("IsReady!");
    }


    public void SetGameMode ( GameMode mode )
    {
        gameMode = mode;
    }

    private void OnDestroy ()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
