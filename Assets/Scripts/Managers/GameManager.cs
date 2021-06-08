using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public static GameState GameState = GameState.WaitingPhase;
    public static Dictionary<string, int> scores = new Dictionary<string, int>();

    public const int NB_CARDS_TO_DISTRIBUTE = 4;

    [SerializeField] Camera cam;
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] GameObject[] objectsToDisableWhenNotPlaying;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Countdown countdown;
    [SerializeField] CactusController cactusController;
    [SerializeField] Scoreboard scoreboard;

    List<Player> players = new List<Player>();
    int currentPlayerIndex = 0;
    int turnsLeft = 0;
    bool isCactus = false;
    bool gameIsFinished = false;

    [System.NonSerialized] public CardController CardDrawn;
    [System.NonSerialized] public CardController CardSelected;
    [System.NonSerialized] public Card CardPlayed;
    [System.NonSerialized] public int NbInstantiatedPlayer = 0;

    public static GameManager Instance { get { return instance; } }
    public List<Player> Players { get { return players; } }
    public Dictionary<string, int> Ccores { get { return scores; } }
    public GameObject[] ObjectsToDisableWhenNotPlaying { get { return objectsToDisableWhenNotPlaying; } }
    public Player CurrentPlayer { get { return players[currentPlayerIndex]; } }
    public bool GameIsFinished { get { return gameIsFinished; } }
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
            DeckManager.Instance.ShuffleDeck();
        }

        HideObjectsToDisableWhenNotPlaying();

        StartCoroutine( WaitUntilAllPlayersAreInstantiated() );
    }

    IEnumerator WaitUntilAllPlayersAreInstantiated()
    {
        yield return new WaitUntil(() => NbInstantiatedPlayer == PhotonNetwork.CurrentRoom.PlayerCount);
        yield return new WaitForEndOfFrame();

        // Set Up player list
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.PlayerList)
        {
            GameObject playerGO = (GameObject)photonPlayer.TagObject;
            players.Add(playerGO.GetComponent<Player>());
        }

        StartCoroutine(WaitUntilHandsAreInitialized());
    }

    IEnumerator WaitUntilHandsAreInitialized()
    {
        GameState = GameState.DristibutingPhase;
        foreach (Player player in Players)
        {
            if (player.photonView.IsMine) {
                player.InitializeHand();
            }
            yield return new WaitUntil( () => player.Cards.Count == NB_CARDS_TO_DISTRIBUTE );
        }
        countdown.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        ShowObjectsToDisableWhenNotPlaying();
        GameState = GameState.DrawingPhase;
        CurrentPlayer.StartTurn();
    }


    public void EndTurn()
    {
        if (isCactus)
        {
            if(turnsLeft == 0)
            {
                FinishGame();
            }
            else
            {
                turnsLeft--;
                cactusController.UpdateTurnsLeft(turnsLeft);
            }
        }

        ShowAllPlayerHands();
        CurrentPlayer.Ui.HideMyTurnGFX();
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count) {
            currentPlayerIndex = 0;
        }
        GameState = GameState.DrawingPhase;
        CurrentPlayer.StartTurn();
    }


    public bool IsMyTurn()
    {
        return CurrentPlayer.PhotonPlayer == PhotonNetwork.LocalPlayer;
    }

    public void SetCardDrawn(CardController cardController)
    {
        if (cardController.View.IsMine)
        {
            HideAllPlayerHandsExeptLocal();
        }
        CardDrawn = cardController;
        GameState = GameState.ReplaceCardPhase;
    }

    public void InstantiateCard(byte eventCode, byte WhereTo, Vector3 position, int? playerViewID = null)
    {
        PhotonNetwork.Instantiate(cardPrefab.name, position, Quaternion.identity, 0, new object[] { eventCode, WhereTo, playerViewID });
    }

    public void HideAllPlayerHandsExeptLocal()
    {
        foreach (Player player in players)
        {
            if ( player != Player.LocalPlayerInstance)
            {
                player.HidePlayerHand();
            }
            else
            {
                player.ShowPlayerHand();
            }
        }
    }

    public void ShowAllPlayerHands()
    {
        foreach (Player player in players)
        {
            player.ShowPlayerHand();
        }
    }

    public void HideLocalPlayerHands()
    {
        foreach (Player player in players)
        {
            if (player != Player.LocalPlayerInstance)
            {
                player.ShowPlayerHand();
            }
            else
            {
                player.HidePlayerHand();
            }
        }
    }

    public void Cactus()
    {
        turnsLeft = Players.Count;
        isCactus = true;
        cactusController.DisableButton();
        EndTurn();
    }

    public void ShowObjectsToDisableWhenNotPlaying()
    {
        foreach(GameObject gameObject in objectsToDisableWhenNotPlaying)
        {
            gameObject.SetActive(true);
        }
    }

    public void HideObjectsToDisableWhenNotPlaying()
    {
        foreach (GameObject gameObject in objectsToDisableWhenNotPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    public void FinishGame()
    {
        gameIsFinished = true;
        HideObjectsToDisableWhenNotPlaying();
        scoreboard.AddScores(players);
        foreach(Player player in players)
        {
            player.ShowAllCardsInHand();
        }
    }


}
