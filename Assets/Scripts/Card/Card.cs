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


    public int Index { get { return index; } }
    public int Value { get { return value; } }
    public CardType Type { get { return type; } }
    public Sprite Front { get { return front; } }
    public Sprite Back { get { return back; } }


    public bool Is7or8 { get { return value == 7 || value == 8; } }
    public bool Is9or10 { get { return value == 9 || value == 10; } }
    public bool IsJackOrQueen { get { return value == 11 || value == 12; } }
    public bool IsBlackKing { get { return value == 13 && (type == CardType.SPADE || type == CardType.CLUB); } }


    public Card(int _index, int _value, CardType _type, Sprite _front, Sprite _back) 
    {
        index = _index;
        value = _value;
        type = _type;
        front = _front;
        back = _back;
    }



}
