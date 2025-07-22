using System;
using System.Collections;
using UnityEngine;

public class NextCard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public IEnumerator nextCard ( Deck deck,CardManager cardManager)
    {
        Card cardComponenet = deck.newCard.GetTheOwner().GetComponent<Card>();
        CardData cardData = cardManager.GetCardData();

        yield return null;
    }
}
