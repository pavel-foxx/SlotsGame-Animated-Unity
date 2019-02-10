using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound_controller : MonoBehaviour {

    public button_handler b_soundOn, b_soundOff;

    private void Awake()
    {
        b_soundOn.OnClick += TurnOn;
        b_soundOff.OnClick += TurnOff;

        if (PlayerPrefs.GetInt("Sound") == 0)
            TurnOn(b_soundOff);
        else
            TurnOff(b_soundOn);
    }

    public void TurnOn(button_handler button)
    {
        PlayerPrefs.SetInt("Sound", 0);
        PlayerPrefs.Save();
        b_soundOff.gameObject.SetActive(true);
        b_soundOn.gameObject.SetActive(false);
        AudioListener.volume = 1;
    }

    public void TurnOff(button_handler button)
    {
        PlayerPrefs.SetInt("Sound", 1);
        PlayerPrefs.Save();
        b_soundOff.gameObject.SetActive(false);
        b_soundOn.gameObject.SetActive(true);
        AudioListener.volume = 0;
    }
}
