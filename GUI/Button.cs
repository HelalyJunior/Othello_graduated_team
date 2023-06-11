using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{

    [SerializeField]
    Slider sliderBlack;

    [SerializeField]
    Slider sliderWhite;

    [SerializeField]
    Slider sliderAlone;
    public void Playervplayer()
    {
        PlayerPrefs.SetInt("mode",0);
        SceneManager.LoadScene("SampleScene");
    }

    public void PlayervAi()
    {
        PlayerPrefs.SetInt("mode",1);
        PlayerPrefs.SetInt("blackai",(int)sliderAlone.value);
        SceneManager.LoadScene("SampleScene");
    }

    public void AivAi()
    {
        PlayerPrefs.SetInt("sliderBlack",(int)sliderBlack.value);
        PlayerPrefs.SetInt("sliderWhite",(int)sliderWhite.value);
        PlayerPrefs.SetInt("mode",2);
        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
