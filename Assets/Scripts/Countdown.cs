using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    [SerializeField] TMP_Text timeLeftText;
    [SerializeField] int time;

    public int Time { get { return time; } }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(StartCountDown());
        HelperManager.Instance.ShowHelper("Memorize your cards");
    }

    IEnumerator StartCountDown()
    {

        Player.LocalPlayerInstance.Cards[0].RevealCard();
        Player.LocalPlayerInstance.Cards[1].RevealCard();


        for (int timeLeft = time; timeLeft > 0; timeLeft--)
        {
            timeLeftText.text = "" + timeLeft;
            yield return new WaitForSeconds(1f);
        }

        timeLeftText.text = "GO";
        yield return new WaitForSeconds(1f);

        Player.LocalPlayerInstance.Cards[0].HideCard();
        Player.LocalPlayerInstance.Cards[1].HideCard();
        GameManager.Instance.StartGame();

        gameObject.SetActive(false);
    }
}
