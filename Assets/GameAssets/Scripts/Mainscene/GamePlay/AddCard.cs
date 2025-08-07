using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class AddCard : MonoBehaviour
{
    public Action OnComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public IEnumerator addNewCard (Deck deck,PoolManager poolManager,Action ChangeCard = null)
    {
        //Debug.Log("Adding new card.");
        Transform transform = deck.newCard.transform;
        GameObject newCard = poolManager.GetFromPool(PoolType.Cards , transform.position , Quaternion.identity , transform);
        newCard.transform.localPosition = Vector3.zero;
        deck.newCard.SetOwner(newCard);
        Card cardComponent = newCard.GetComponent<Card>();
        cardComponent.HideMask();
       //Debug.Log(cardComponent.name);
        Sequence AddSequence = DOTween.Sequence();
        Sequence sequence = DOTween.Sequence();

        Vector3 midRotation = new Vector3(0 , -90f , 0);
        Vector3 endRoatation = new Vector3(0 , 0 , 0);
        Vector3 punchPosition = new Vector3(0 , -25f , 0);
        CommandCenter.Instance.soundManager_.PlaySound("Flip");
        AddSequence.Join(sequence.Append(newCard.transform.DOLocalRotate(midRotation , .25f).SetEase(Ease.InOutSine))
                .AppendCallback(() =>
                {
                    //Get the card component and show it
                    //Debug.Log("Set New Card!");
                    ChangeCard?.Invoke();

                    //Debug.Log(cardComponent.name);

                    if (cardComponent != null)
                    {
                        cardComponent.ShowCard();
                        cardComponent.hideCardBg();
                        cardComponent.HideCardOutline();
                    }
                })
                .Append(newCard.transform.DOLocalRotate(endRoatation , .25f).SetEase(Ease.InOutSine)));

        AddSequence.Join(newCard.transform.DOPunchPosition(punchPosition , .5f , 0 , 0).SetEase(Ease.InOutSine));

        AddSequence.OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
        yield return null;
    }
}
