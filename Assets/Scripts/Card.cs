using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardType { CLUB, DIAMOND, HEART, SPADE }

public class Card
{

    int value;
    CardType type;
    Sprite front;
    Sprite back;

    public int Value { get { return value; } }
    public CardType Type { get { return type; } }
    public Sprite Front { get { return front; } }
    public Sprite Back { get { return back; } }


    public Card(int _value, CardType _type, Sprite _front, Sprite _back) 
    {
        value = _value;
        type = _type;
        front = _front;
        back = _back;
    }


}
