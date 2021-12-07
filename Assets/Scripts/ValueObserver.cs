using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueObserver : MonoBehaviour
{
    public Text m_mode;
    public Text m_phase;
    public Text m_turn;
    public Text mouseTestTxt;


    public static bool mouseTest = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_mode.text = "NowMode:\t" + GameManager.Instance.m_gameStatus.GetNowMode().ToString();
        m_phase.text = "NowPhase:\t" + GameManager.Instance.m_gameStatus.m_gamePhase.ToString();
        m_turn.text = "Turn:\t" + GameManager.Instance.m_gameStatus.m_turn;
        mouseTestTxt.text = "mouseTest:\t" + mouseTest;
    }
}
