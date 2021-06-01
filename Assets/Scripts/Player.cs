using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Player: MonoBehaviourPunCallbacks
{

    List<Card> cards = new List<Card>();
    PlayerHand hand;
    PhotonPlayer photonPlayer;


    public static GameObject LocalPlayerInstance;
    public PhotonPlayer PhotonPlayer { get { return photonPlayer; } set { photonPlayer = value; } }
    public PlayerHand Hand { get { return hand; } set { hand = value; } }

    /*public delegate void OnCardsChangedDelegate();
    public OnCardsChangedDelegate onCardsChangedCallback;*/


    void Awake()
    {
        // #Important used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        // #Critical we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //player.Hand = GameManager.Instance.playerHands.Last();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard(Card card)
    {
        cards.Add(card);
        CardController cardController = Instantiate(GameManager.Instance.CardPrefab, hand.transform).GetComponent<CardController>();
        cardController.SetUp(card);
        FinishTurn();
    }

    public void FinishTurn()
    {
        if(!photonView.IsMine || !RoomManager.Instance.IsMyTurn()) {
            return;
        }

        object[] data = null;
        PhotonNetwork.RaiseEvent(RoomManager.END_PLAYER_TURN, data, RaiseEventOptions.Default, SendOptions.SendReliable);
    }
}
