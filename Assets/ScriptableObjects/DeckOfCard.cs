using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptables/DeckOfCard")]
public class DeckOfCard : ScriptableObject
{

    [SerializeField] Sprite[] cardsSprite;
    [SerializeField] Sprite backOfCardSprite;

    public Sprite[] CardsSprite { get { return cardsSprite; } }
    public Sprite BackOfCardSprite { get { return backOfCardSprite; } }

}
