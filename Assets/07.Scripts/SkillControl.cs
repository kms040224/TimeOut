using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillControl : MonoBehaviour
{
    public GameObject[] hideSkillButtons;
    public GameObject[] textpros;
    public Text hideSkillTimeTexts;
    public Image[] hideSkillImages;
    private bool[] isHideSkills = { false, false, false, false };
    private float[] skillTimes = { 3, 6, 9, 12 };
    private float[] getSkillTimes = { 0, 0, 0, 0 };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
