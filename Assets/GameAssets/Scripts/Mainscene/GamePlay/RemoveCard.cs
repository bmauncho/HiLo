using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RemoveCard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public IEnumerator removeCurrentCard (Deck deck,PoolManager poolManager)
    {
        //1.move to end point
        //Debug.Log("Removing current card.");
        Vector3 endPoint = new Vector3(-300f , -75f , 0);
        Vector3 endRotation = new Vector3(0 , 0 , 45f);

        GameObject card = deck.newCard.GetTheOwner();
        deck.newCard.RemoveOwner();
        deck.prevCard.SetOwner(card);
        card.transform.SetParent(deck.prevCard.transform);

        Sequence sequence = DOTween.Sequence();

        sequence.Join(card.transform.DOLocalMove(endPoint , .5f).SetEase(Ease.InOutSine));
        sequence.Join(card.transform.DOLocalRotate(endRotation , .5f).SetEase(Ease.InOutSine));

        Card cardComponent = card.GetComponent<Card>();
        cardComponent.ShowMask();

        sequence.OnComplete(() =>
        {
            //Debug.Log("Card moved to end point and rotated.");
            //2. return to pool
            poolManager.ReturnToPool(PoolType.Cards , card);
            deck.prevCard.RemoveOwner();
        });

        yield return null;
    }
}
