using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CardController : MonoBehaviour, IPunInstantiateMagicCallback
{


    Card card;
    Player owner;

    SpriteRenderer spriteRenderer;
    PhotonView photonView;


    [SerializeField] GameObject selectedGFX;

    public Card Card { get { return card; } }
    public Player Owner { get { return owner; } }
    public PhotonView View { get { return photonView; } }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        photonView = this.GetComponent<PhotonView>();

        byte eventCode = (byte)info.photonView.InstantiationData[0];
        byte whereToCode = (byte)info.photonView.InstantiationData[1];
        if (eventCode == EventCode.DRAW_CARD_FROM_DECK)
        {
            card = DeckManager.Instance.DrawTopCard();
        } 
        else if (eventCode == EventCode.DRAW_CARD_FROM_DISCARD_PILE)
        {
            card = DiscardPileManager.Instance.DrawTopCard();
        }

        spriteRenderer.sprite = card.Front;
        this.gameObject.name = "CARD_" + card.Type + "_" + card.Value;


        if (whereToCode == EventCode.DRAW_TO_HAND)
        {
            int? viewId = (int?)info.photonView.InstantiationData[2];
            if (viewId != null)
            {
                PhotonView view = PhotonView.Find( (int) viewId);
                Player player = view.GetComponent<Player>();
                player.AddCard(this);
            }
        }
        else
        {
            if (!photonView.IsMine)
            {
                HideCard();
            }
            GameManager.Instance.SetCardDrawn(this);
        }

    }


    public void RevealCard()
    {
        spriteRenderer.sprite = card.Front;
    }

    public void HideCard()
    {
        spriteRenderer.sprite = card.Back;
    }

    public void SelectCard()
    {
        if(GameManager.Instance.CardSelected != null) {
            GameManager.Instance.CardSelected.UnselectCard();
        }
        selectedGFX.SetActive(true);
        GameManager.Instance.CardSelected = this;
    }

    public void UnselectCard()
    {
        selectedGFX.SetActive(false);
        GameManager.Instance.CardSelected = null;

    }


    public IEnumerator RevealCardForSeconds(float seconds)
    {
        RevealCard();
        yield return new WaitForSeconds(seconds);
        HideCard();
        PhotonNetwork.RaiseEvent(EventCode.END_PLAYER_TURN, null, RaiseEventOptions.Default, SendOptions.SendReliable);
        GameManager.Instance.EndTurn();
    }

    public void SetUpOwner(Player player)
    {
        transform.SetParent(player.Hand.transform, false);
        HideCard();
        owner = player;
    }

    public void DestroySelf()
    {
        if(owner != null)
            owner.RemoveCard(this);
        Destroy(this.gameObject);
    }


    public void Effect()
    {
        if (!photonView.IsMine)
            return;


        GameManager.GameState = GameState.PlayCardEffectPhase;
        if (card.Is7or8)
        {
            HelperManager.Instance.ShowHelper("Click on one of your card to reveal");
            GameManager.Instance.CardPlayed = card;
            GameManager.Instance.HideAllPlayerHandsExeptLocal();
        }
        else if (card.Is9or10)
        {
            HelperManager.Instance.ShowHelper("Click on another player card to reveal");
            GameManager.Instance.CardPlayed = card;
            GameManager.Instance.HideLocalPlayerHands();

        }
        else if (card.IsJackOrQueen || card.IsBlackKing)
        {
            if (card.IsJackOrQueen)
                HelperManager.Instance.ShowHelper("Click on another player card to steal");
            else if (card.IsBlackKing)
                HelperManager.Instance.ShowHelper("Click on another player card to reveal and steal");
            GameManager.Instance.CardPlayed = card;
            GameManager.Instance.HideLocalPlayerHands();
        }
        else
        {
            PhotonNetwork.RaiseEvent(EventCode.END_PLAYER_TURN, null, RaiseEventOptions.Default, SendOptions.SendReliable);
            GameManager.Instance.EndTurn();
        }
    }


    public void ReturnToPos()
    {
        transform.position = this.GetComponent<Draggable>().PositionToReturnTo;
    }
}
