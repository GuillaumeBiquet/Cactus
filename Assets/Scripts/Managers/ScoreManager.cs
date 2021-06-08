using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance { get { return instance; } }

    public static Dictionary<string, int> Scores = new Dictionary<string, int>();


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Scores.Add(PhotonNetwork.PlayerList[i].NickName, 0);
            }
        }
    }

}
