using TMPro;
using UnityEngine;

public class ApiManager : MonoBehaviour
{
    [SerializeField] private string Player_Id = string.Empty;
    [SerializeField] private string Game_Id = string.Empty;
    [SerializeField] private string Client_id = string.Empty;
    [SerializeField] private string CashAmount = string.Empty;
    [SerializeField] private string BetId = string.Empty;
    [SerializeField] private string BetAmount = string.Empty;
    public PlaceBet placeBet;
    public UpdateBet updateBet;
    public StartApi StartApi;
    public GuessApi guessApi;
    public SkipApi SkipApi;
    public CashOutApi cashOutApi;
    public PreviewSkipApi previewSkipApi;
    public bool IsFirstPlayDone = false;
    public TMP_Text transactiontext;
    private void Awake ()
    {
        SetUp();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.AddTransactionText(transactiontext);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp ()
    {
        Player_Id = GameManager.Instance.GetPlayerId();
        Game_Id = GameManager.Instance.GetGameId();
        Client_id = GameManager.Instance.GetClientId();
        CashAmount = GameManager.Instance.GetCashAmount();
        //Debug.Log("SetUpDone!");
    }

    public string SetBetId ()
    {
        int id = Random.Range(1 , 1000000000);
        string betId = ConfigMan.Instance.GetBetId();
        BetId = betId;
        return id.ToString();
    }

    public void GetBetAmount (string amount)
    {
        BetAmount = amount;
    }

    public string GetPlayerId ()
    {
        return Player_Id;
    }

    public string GetGameId ()
    {
        return Game_Id;
    }

    public string GetClientId ()
    {
        return Client_id;
    }

    public string GetBetAmountValue ()
    {
        return BetAmount;
    }

    public string GetBetId ()
    {
        return BetId;
    }
    [ContextMenu("Text Bet Id")]
    public void TestBetId ()
    {
        BetId = SetBetId();
    }

    public string GetCashAmount ()
    {
        return CashAmount;
    }

    public void SetIsFirstPlayDone(bool isFirstPlayDone )
    {
        IsFirstPlayDone = isFirstPlayDone;
    }
}
