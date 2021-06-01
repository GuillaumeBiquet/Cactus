using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    PhotonPlayer photonPlayer;

    public PhotonPlayer PhotonPlayer { get { return photonPlayer; } }

    public void SetPlayerInfo(PhotonPlayer _photonPlayer)
    {
        photonPlayer = _photonPlayer;
        GetComponent<TMP_Text>().text = _photonPlayer.NickName;
    }
}
