using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] GameObject myTurnGFX;
    Player target;

    public void SetTarget(Player _target)
    {
        // Cache references for efficiency
        target = _target;
        playerNameText.text = target.PhotonPlayer.NickName;
        GetComponent<Canvas>().worldCamera = GameManager.Instance.Cam;
    }

    public void SetUIBackgroundColor(Color backgroundColor)
    {
        Image img = this.GetComponent<Image>();
        img.color = backgroundColor;
    }


    public void ShowMyTurnGFX()
    {
        myTurnGFX.SetActive(true);
    }

    public void HideMyTurnGFX()
    {
        myTurnGFX.SetActive(false);
    }
}
