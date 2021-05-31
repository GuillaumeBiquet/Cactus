using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{


    [SerializeField] Image image;

    bool isFlipped = false;

    Card card;
    public Card Card { get { return card; } }

    public void SetUp(Card _card)
    {
        card = _card;
        image.sprite = card.Front;
        this.gameObject.name = "CARD_" + card.Type + "_" + card.Value;
    }

    void Update()
    {

        if ((this.transform.eulerAngles.y > 90f || this.transform.eulerAngles.y < -90f) && !isFlipped)
        {
            image.sprite = card.Back;
            isFlipped = true;
        }
        else if ((this.transform.eulerAngles.y > -90f && this.transform.eulerAngles.y < 90f) && isFlipped)
        {
            image.sprite = card.Front;
            isFlipped = false;
        }

    }

}
