using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class EventCode
{
    public const byte END_PLAYER_TURN = 1;
    public const byte DRAW_CARD_FROM_DECK = 2;
    public const byte PLAY_CARD = 3;
    public const byte SET_UP_DECK = 4;
    public const byte DRAW_CARD_FROM_DISCARD_PILE = 5;
    public const byte DISCARD_CARD = 6;
}

public enum TurnState
{
    DrawingPhase,
    Play,

}


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] Camera camera;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<PlayerHand> playerHands;

    public static List<PlayerHand> PlayerHands;
    public static int NumberOfInstantiatedPlayer = 0;


    List<Player> players = new List<Player>();

    int currentPlayerIndex = 0;

    public List<Player> Players { get { return players; } }
    public Player CurrentPlayer { get { return players[currentPlayerIndex]; } }




    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        PlayerHands = new List<PlayerHand>(playerHands);
    }

    // Start is called before the first frame update
    void Start()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Lobby");
            return;
        }


        if (Player.LocalPlayerInstance == null)
        {
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        }

        //is Master
        if (PhotonNetwork.IsMasterClient)
        {
            // suffle deck
            DeckManager.Instance.ShuffleDeck();
        }

        StartCoroutine( WaitUntilAllPlayersAreInstantiated() );


    }

    IEnumerator WaitUntilAllPlayersAreInstantiated()
    {
        yield return new WaitUntil(() => NumberOfInstantiatedPlayer == PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.LogError("nb instantiated players = " + NumberOfInstantiatedPlayer + " / nb players in room = " + PhotonNetwork.CurrentRoom.PlayerCount);
        yield return new WaitForEndOfFrame();

        SetUpPlayerList();
        SetUpPlayerHands();

        StartCoroutine(InitializeHands());
    }

    IEnumerator InitializeHands()
    {
        int nbCardToDistribute = 4;
        while(CurrentPlayer.Cards.Count < nbCardToDistribute)
        {
            int nbCardDrawn = CurrentPlayer.Cards.Count;
            Player oldPlayer = CurrentPlayer;
            DeckManager.Instance.InstantiateCard();
            yield return new WaitUntil(() => oldPlayer.Cards.Count == (nbCardDrawn + 1) );
        }
    }


    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }


    void OnEvent(EventData eventData)
    {
        byte eventCode = eventData.Code;
        if (eventCode == EventCode.END_PLAYER_TURN)
        {
            EndTurn();
        }
        /*else if (eventCode == EventCode.DRAW_CARD_FROM_DECK)
        {
            DeckManager.Instance.DrawTopCard();
        }*/
        else if (eventCode == EventCode.SET_UP_DECK)
        {
            DeckManager.Instance.SetUpDeck((object[])eventData.CustomData);
        }
        else if (eventCode == EventCode.DISCARD_CARD)
        {
            object[] data = (object[])eventData.CustomData;
            PhotonView view = PhotonView.Find( (int)data[0] );
            CardController cardController = view.GetComponent<CardController>();
            DiscardPileManager.Instance.Discard(cardController);
        }
    }

    public void EndTurn()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
            //roomTurn++;
        }
    }


    public bool IsMyTurn()
    {
        return CurrentPlayer.PhotonPlayer == PhotonNetwork.LocalPlayer;
    }


    void SetUpPlayerList()
    {
        Vector3 cameraRotation = new Vector3(0, 0, 360);
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.PlayerList)
        {
            cameraRotation.z += 90;
            if (photonPlayer.IsLocal) {
                camera.transform.Rotate(cameraRotation);
            }
            players.Add( GetPlayerWithPhotonPlayer(photonPlayer) );
        }
    }

    void SetUpPlayerHands()
    {
        foreach (Player player in players)
        {
            player.SetUpHand(PlayerHands.Last());
            PlayerHands.Remove(PlayerHands.Last());
        }
    }

    Player GetPlayerWithPhotonPlayer(PhotonPlayer photonPlayer)
    {
        GameObject playerGO = (GameObject)photonPlayer.TagObject;
        return playerGO.GetComponent<Player>();

    }
}
