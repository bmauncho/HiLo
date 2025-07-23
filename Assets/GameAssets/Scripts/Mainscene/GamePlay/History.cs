using UnityEngine;
using UnityEngine.UI;
public class History : MonoBehaviour
{
    [Header("Images")]
    public Image Outline;
    public Image cardRank;
    public Image cardSuite_Icon;
    public Image cardSuite_Bg;
    public Image Bet_History;

    public GameObject BetHistory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetCardHistory(
       Sprite outline,
       Sprite rank,
       Sprite suite,
       Sprite suite_Bg,
       Sprite bet_History=null,
       bool canShowHistory = true )
    {
        Outline.sprite = outline;
        cardRank.sprite = rank;
        cardSuite_Icon.sprite = suite;
        cardSuite_Bg.sprite = suite_Bg;
        if(!canShowHistory)
        {
            BetHistory.SetActive(false);
        }
        else
        {
            BetHistory.SetActive(true);
            Bet_History.sprite = bet_History;
        }
    }
}
