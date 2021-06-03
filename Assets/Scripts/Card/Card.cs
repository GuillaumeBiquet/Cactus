using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardType { CLUB, DIAMOND, HEART, SPADE }

[Serializable]
public class Card
{

    int index;
    int value;
    CardType type;
    Sprite front;
    Sprite back;

    CardController cardController;

    public int Index { get { return index; } }
    public int Value { get { return value; } }
    public CardType Type { get { return type; } }
    public Sprite Front { get { return front; } }
    public Sprite Back { get { return back; } }
    public CardController CardController { get { return cardController; } set { cardController = value; } }


    public Card(int _index, int _value, CardType _type, Sprite _front, Sprite _back) 
    {
        index = _index;
        value = _value;
        type = _type;
        front = _front;
        back = _back;
    }



}
