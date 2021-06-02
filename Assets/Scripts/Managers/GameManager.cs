using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<PlayerHand> playerHands;

    [SerializeField] public GameObject CardPrefab;
    [SerializeField] public Canvas Canvas;

    public static List<PlayerHand> PlayerHands;
    public static int NumberOfInstantiatedPlayer = 0;


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
            Player player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
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
        Debug.LogError("n = " + NumberOfInstantiatedPlayer + " / n = " + PhotonNetwork.CurrentRoom.PlayerCount);

        yield return new WaitForEndOfFrame();

        RoomManager.Instance.SetUpPlayerList();

        foreach(Player player in RoomManager.Instance.Players)
        {          
            player.SetUpHand(PlayerHands.Last());
            PlayerHands.Remove(PlayerHands.Last());
        }

    }

}
