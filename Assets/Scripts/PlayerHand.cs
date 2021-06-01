using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{

    Player player;
    public Player Player { get { return player; } set { player = value; } }

    // Start is called before the first frame update
    void Start()
    {
        //player.onCardsChangedCallback += UpdateCardsHUD;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void UpdateCardsHUD()
    {
        Debug.Log("here");
    }

}
