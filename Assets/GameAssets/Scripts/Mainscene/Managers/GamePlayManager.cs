using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public GamePlay gamePlay;
    public Skips Skips;
    public Deck deck;

    public void Start ()
    {
        Invoke(nameof(SetActiveCard),.25f);
    }

    public void SetActiveCard ()
    {
        Transform transform = deck.newCard.transform;
        GameObject card = CommandCenter.Instance.poolManager_.GetFromPool(PoolType.Cards , transform.position , Quaternion.identity,transform);
        deck.newCard.SetOwner(card);
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent != null)
        {
            cardComponent.ShowCard();
            cardComponent.hideCardBg();
            cardComponent.HideCardOutline();
        }
    }
    public void ToggleGamePlay ()
    {
        gamePlay.ToggleGamePlay();
    }

    public void SetCashOutAmount ( string amount )
    {
        gamePlay.SetCashOutAmount(amount);
    }
}
