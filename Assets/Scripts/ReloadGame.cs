using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadGame : MonoBehaviour
{
    //Used because PhotonNetwork.LoadLevel() can't load the same scene on clients


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LoadLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
