using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
        player.onCardsChangedCallback += UpdateCardsHUD;
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
