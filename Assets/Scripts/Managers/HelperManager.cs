using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelperManager : MonoBehaviour
{

    private static HelperManager instance;
    public static HelperManager Instance { get { return instance; } }

    [SerializeField] TMP_Text helper;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    public void ShowHelper(string help)
    {
        helper.text = help;
        helper.gameObject.SetActive(true);
    }

    public void HideHelper()
    {
        helper.text = "";
        helper.gameObject.SetActive(false);
    }

}
