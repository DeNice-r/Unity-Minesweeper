using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Text rec;

    private void Start()
    {
        if(PlayerPrefs.GetFloat("record") < 1)
        PlayerPrefs.SetFloat("record", 999.999f);
        rec.text = PlayerPrefs.GetFloat("record").ToString();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
