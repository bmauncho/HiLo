using UnityEngine;

public class ApiManager : MonoBehaviour
{
    [SerializeField] private string Player_Id = string.Empty;
    [SerializeField] private string Game_Id = string.Empty;
    [SerializeField] private string Client_id = string.Empty;
    [SerializeField] private string CashAmount = string.Empty;
    [SerializeField] private string BetId = string.Empty;
    private void Awake ()
    {
        SetUp();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
        Debug.Log("SetUpDone!");
    }

    public string GetBetId ()
    {
        int id = Random.Range(1 , 1000000000);
        string betId = ConfigMan.Instance.GetBetId();
        return id.ToString();
    }
}
