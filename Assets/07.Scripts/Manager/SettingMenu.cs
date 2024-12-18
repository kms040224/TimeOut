using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject howtoplayPanel;

    private void Start()
    {
        settingsPanel.SetActive(false);
        howtoplayPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenHowtoPlayPanel()
    {
        howtoplayPanel.SetActive(true);
    }

    public  void CloseHowtoPlayPanel()
    {
        howtoplayPanel.SetActive(false);
    }
}
