using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{

    Player player;
    public Player Player { get { return player; } set { player = value; } }

    [SerializeField] int columnLength = 7;
    [SerializeField] Vector2 space = new Vector2(0.4f, 0.5f);

    Vector3 position = Vector3.zero;
    Vector2 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        //player.onCardsChangedCallback += UpdateCardsHUD;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
    }

    public void UpdateCardsPosition(List<Card> cards)
    {
        int nbColumn = Mathf.Clamp(cards.Count, 1, columnLength);
        int i = 0;
        startPosition.x = -space.x * Mathf.FloorToInt(nbColumn * .5f);
        if (nbColumn % 2 == 0)
        {
            startPosition.x += (space.x * .5f);
        }

        foreach (Card card in cards)
        {
            position.x = startPosition.x + (space.x * (i % nbColumn));
            position.y = startPosition.y + (space.y * (i / nbColumn));
            card.CardController.transform.localPosition = position;
            i++;
        }

    }

}
