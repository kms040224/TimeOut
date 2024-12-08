using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public GameObject AttributePanel;

    private void Start()
    {
        AttributePanel.SetActive(false);
    }
    public void OpenAttributePanel()
    {
        AttributePanel.SetActive(true);
    }
}
