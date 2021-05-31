using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{

    private List<Card> cards = new List<Card>();
    [SerializeField] Transform hand;

    public delegate void OnCardsChangedDelegate();
    public OnCardsChangedDelegate onCardsChangedCallback;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard(Card card)
    {
        cards.Add(card);
        CardController cardController = Instantiate(GameManager.Instance.cardPrefab, hand).GetComponent<CardController>();
        cardController.SetUp(card);
    }
}
