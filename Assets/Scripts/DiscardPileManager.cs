using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class DiscardPileManager : MonoBehaviour, IDropHandler
{
    List<Card> discardCards = new List<Card>();
    [SerializeField] Image LastDiscardedCard;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePileImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag.name + " was dropped");

        CardController discardCard = eventData.pointerDrag.GetComponent<CardController>();
        discardCards.Add(discardCard.Card);
        Destroy(eventData.pointerDrag);

        UpdatePileImage();
    }

    private void UpdatePileImage()
    {
        if(discardCards.Count < 1)
        {
            LastDiscardedCard.enabled = false;
            return;
        }
        else if (!LastDiscardedCard.enabled)
        {
            LastDiscardedCard.enabled = true;
        }
        LastDiscardedCard.sprite = discardCards.Last().Front;
    }

    public void DrawCard()
    {
        if (discardCards.Count < 1)
        {
            Debug.Log("Discard pile is empty");
            return;
        }

        Card cardToDraw = discardCards.Last();
        GameManager.Instance.player.DrawCard(cardToDraw);
        discardCards.Remove(cardToDraw);

        UpdatePileImage();
    }

}
