using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RetainCard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public IEnumerator loseAnim ( Deck deck )
    {
        GameObject card = deck.newCard.GetTheOwner();
        if (card == null)
        {
            Debug.LogError("Card is null");
            yield break;
        }
       
        Card cardComponent = card.GetComponent<Card>();
        cardComponent.ShowCardOutline();
        yield return StartCoroutine(Swing(card.transform,15f,10));

        Debug.Log("loseAnim Played!");
    }


    public IEnumerator Swing ( Transform target , float swingAngle = 15f , float swingSpeed = 2f )
    {
        float elapsed = 0f;
        float totalDuration = ( Mathf.PI / swingSpeed ) * 4f; // Two full swings
        float startTime = Time.time;

        while (elapsed < totalDuration)
        {
            elapsed = Time.time - startTime;

            // Damping: gradually reduce the swing angle as time progresses
            float damping = Mathf.Lerp(1f , 0f , elapsed / totalDuration); // Linearly decreases from 1 to 0
            float angle = swingAngle * Mathf.Sin(elapsed * swingSpeed) * damping;

            target.rotation = Quaternion.Euler(0f , 0f , angle);

            yield return null;
        }

        // Reset to neutral rotation at end
        target.rotation = Quaternion.Euler(Vector3.zero);
    }
}
