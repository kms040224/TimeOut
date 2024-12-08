using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public GameObject AttributePanel;

    public void OpenAttributePanel()
    {
        AttributePanel.SetActive(true);
    }
}
