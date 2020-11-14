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
        //PlayerPrefs.SetFloat("record", 1000f);
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
