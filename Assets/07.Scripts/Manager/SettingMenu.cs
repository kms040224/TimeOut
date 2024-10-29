using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public GameObject settingsPanel;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
