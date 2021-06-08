using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NicknameMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField nickNameInput;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickValidate()
    {
        if (nickNameInput.text == "")
        {
            return;
        }

        LobbyManager.Instance.ConnectToMaster(nickNameInput.text);
    }
}
