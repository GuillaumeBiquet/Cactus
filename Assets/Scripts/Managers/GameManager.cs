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

    public List<PlayerHand> PlayerHands { get { return playerHands; } }


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


        if (Player.LocalPlayerInstance == null)
        {


            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            Player player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();

            PlayerHand playerHand = playerHands.Last();
            player.Hand = playerHand;
            playerHand.Player = player;
            playerHand.gameObject.SetActive(true);
            playerHands.Remove(playerHand);

            RoomManager.Instance.AddPlayer(player);
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }

    }
}
