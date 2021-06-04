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
    public const byte REPLACE_CARD = 7;
}

public enum GameState
{
    DristibutingPhase,
    WaitingPhase,
    DrawingPhase,
    ReplaceCardPhase,
    PlayCardEffectPhase
}


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] Camera cam;
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] GameObject cardPrefab;

    List<Player> players = new List<Player>();
    int currentPlayerIndex = 0;
    CardController cardDrawn;

    public static int NumberOfInstantiatedPlayer = 0;
    public static GameState GameState = GameState.WaitingPhase;

    public List<Player> Players { get { return players; } }
    public Player CurrentPlayer { get { return players[currentPlayerIndex]; } }
    public CardController CardDrawn { get { return cardDrawn; } }
    public Camera Cam { get { return cam; } }


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


        for (int i=0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].IsLocal)
            {
                // spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(playerPrefabs[i].name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
            }

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
        Debug.Log("nb instantiated players = " + NumberOfInstantiatedPlayer + " / nb players in room = " + PhotonNetwork.CurrentRoom.PlayerCount);
        yield return new WaitForEndOfFrame();


        // Set Up player list
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.PlayerList)
        {
            GameObject playerGO = (GameObject)photonPlayer.TagObject;
            players.Add(playerGO.GetComponent<Player>());
        }

        StartCoroutine(InitializeHands());
    }

    IEnumerator InitializeHands()
    {
        GameState = GameState.DristibutingPhase;

        int nbCardToDistribute = 4;
        while(CurrentPlayer.Cards.Count < nbCardToDistribute)
        {
            int nbCardDrawn = CurrentPlayer.Cards.Count;
            Player oldPlayer = CurrentPlayer;
            if (IsMyTurn()) { 
                InstantiateCard(EventCode.DRAW_CARD_FROM_DECK, Vector3.zero);
            }
            yield return new WaitUntil(() => oldPlayer.Cards.Count == (nbCardDrawn + 1) );
        }

        GameState = GameState.DrawingPhase;
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
        else if (eventCode == EventCode.REPLACE_CARD)
        {
            object[] data = (object[])eventData.CustomData;
            PhotonView view = PhotonView.Find((int)data[0]);
            CardController cardController = view.GetComponent<CardController>();
            CurrentPlayer.ReplaceCard(cardController, cardDrawn);
        }
    }

    public void EndTurn()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
        }

        if(GameState != GameState.DristibutingPhase)
        {
            GameState = GameState.DrawingPhase;
        }
    }


    public bool IsMyTurn()
    {
        return CurrentPlayer.PhotonPlayer == PhotonNetwork.LocalPlayer;
    }

    public void SetCardDrawn(CardController cardController)
    {
        cardDrawn = cardController;
        GameState = GameState.ReplaceCardPhase;
    }

    public void InstantiateCard(byte eventCode, Vector3 position)
    {
        PhotonNetwork.Instantiate(cardPrefab.name, position, Quaternion.identity, 0, new object[] { eventCode });
    }




}
