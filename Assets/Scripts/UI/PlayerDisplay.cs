using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    PhotonPlayer photonPlayer;
    [SerializeField] TMP_Text playerName;

    public PhotonPlayer PhotonPlayer { get { return photonPlayer; } }

    public void SetPlayerInfo(PhotonPlayer _photonPlayer)
    {
        photonPlayer = _photonPlayer;
        playerName.text = _photonPlayer.NickName;
    }
}
