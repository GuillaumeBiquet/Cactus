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

 
    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
    }

    public void UpdateCardsPosition(List<CardController> cardControllerList)
    {
        int nbColumn = Mathf.Clamp(cardControllerList.Count, 1, columnLength);
        int nbRow = Mathf.CeilToInt( (float) cardControllerList.Count / (float) columnLength);
        startPosition.x = -space.x * Mathf.FloorToInt(nbColumn * .5f);
        startPosition.y = -space.y * Mathf.FloorToInt(nbRow * .5f);

        if (nbColumn % 2 == 0)
        {
            startPosition.x += (space.x * .5f);
        }

        if (nbRow % 2 == 0)
        {
            startPosition.y += (space.y * .5f);
        }

        int i = 0;
        foreach (CardController cardController in cardControllerList)
        {
            position.x = startPosition.x + (space.x * (i % nbColumn));
            position.y = startPosition.y + (space.y * (i / nbColumn));
            cardController.transform.localPosition = position;
            i++;
        }

    }

}
