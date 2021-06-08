using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CactusController : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void OnClick()
    {
        if (GameManager.GameState == GameState.DrawingPhase && GameManager.Instance.IsMyTurn())
        {
            PhotonNetwork.RaiseEvent(EventCode.CACTUS, null, RaiseEventOptions.Default, SendOptions.SendReliable);
            GameManager.Instance.Cactus();
        }
    }

    public void DisableButton()
    {
        this.GetComponent<Button>().interactable = false;
        text.gameObject.SetActive(true);
    }

    public void UpdateTurnsLeft(int nbTurnLeft)
    {
        if(nbTurnLeft == 0)
        {
            text.text = "Last turn !";
        }
        else
        {
            text.text = nbTurnLeft + " turn(s) left";
        }
    }

}
