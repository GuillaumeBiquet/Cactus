using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{

    [SerializeField] TMP_Text[] texts;
    [SerializeField] GameObject newGameButton;
    [SerializeField] GameObject panel;


    private void Start()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            texts[i].gameObject.SetActive(true);
        }
        UpdateScoresText();
    }

    void UpdateScoresText()
    {
        int i = 0;
        foreach (KeyValuePair<string, int> score in ScoreManager.Scores)
        {
            texts[i].text = score.Key + ": " + score.Value;
            i++;
        }
    }

    public void AddScores(List<Player> players)
    {
        foreach (Player player in players)
        {
            ScoreManager.Scores[player.PhotonPlayer.NickName] += CalculatePlayerScore(player);
        }
        UpdateScoresText();
        Open();
    }

    int CalculatePlayerScore(Player player)
    {
        int playerScore = 0;
        foreach (CardController card in player.Cards)
        {
            playerScore += CardScore(card.Card);
        }
        return playerScore;
    }

    int CardScore(Card card)
    {
        if (card.IsJackOrQueen)
        {
            return 10;
        }
        else if (card.Value == 13)
        {
            if (card.IsBlackKing)
            {
                return 15;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return card.Value;
        }
    }

    public void NewGame()
    {
        if (PhotonNetwork.IsMasterClient && GameManager.Instance.GameIsFinished)
        {
            PhotonNetwork.RaiseEvent(EventCode.RELOAD, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
    }

    public void Close()
    {
        newGameButton.SetActive(false);
        this.GetComponent<Image>().enabled = false;
        panel.SetActive(false);
    }

    public void Open()
    {
        if (PhotonNetwork.IsMasterClient && GameManager.Instance.GameIsFinished)
        {
            newGameButton.SetActive(true);
        }
        this.GetComponent<Image>().enabled = true;
        panel.SetActive(true);
    }
}
