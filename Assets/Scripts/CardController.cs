using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CardController : MonoBehaviourPunCallbacks, IPunObservable
{


    [SerializeField] Image image;

    bool isFlipped = false;

    Card card;
    public Card Card { get { return card; } }

    private Player owner;

    public void SetUp(Card _card, Player player)
    {
        card = _card;
        image.sprite = card.Front;
        this.gameObject.name = "CARD_" + card.Type + "_" + card.Value;
        owner = player;

        PhotonNetwork.AllocateViewID(photonView);
        Debug.LogError(photonView.ViewID);
        if (owner.photonView.IsMine)
        {
            photonView.TransferOwnership(owner.PhotonPlayer);
        }

    }

    void Update()
    {

        if ((this.transform.eulerAngles.y > 90f || this.transform.eulerAngles.y < -90f) && !isFlipped)
        {
            image.sprite = card.Back;
            isFlipped = true;
        }
        else if ((this.transform.eulerAngles.y > -90f && this.transform.eulerAngles.y < 90f) && isFlipped)
        {
            image.sprite = card.Front;
            isFlipped = false;
        }

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }

}
