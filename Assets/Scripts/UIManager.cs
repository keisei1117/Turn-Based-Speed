using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text m_myOrOppoTurn;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        m_myOrOppoTurn.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        m_myOrOppoTurn.enabled = GameManager.Instance.m_gameStatus.m_nowMode == GameStatus.Mode.PLAYING;

        if (GameManager.Instance.m_gameStatus.IsMyTurn())
        {
            m_myOrOppoTurn.text = "Your Turn";
        }
        else
        {
            m_myOrOppoTurn.text = "Opponet's Turn";
        }
    }

    public GameObject m_TurnEndButtonObj;
    public Action m_turnEndButtonClickedBehave;
    public void TurnEndButtonClicked()
    {
        Debug.Log("TurnEndButtonClicked");
        if(m_turnEndButtonClickedBehave != null)
            m_turnEndButtonClickedBehave();
    }
    public void EnableTurnEndButton()
    {
        m_TurnEndButtonObj.SetActive(true);
        //m_TurnEndButtonObj.GetComponent<Image>().enabled = false;
        //m_TurnEndButtonObj.GetComponentInChildren<Text>().enabled = false;
    }
    public void DisableTurnEndButton()
    {
        m_TurnEndButtonObj.SetActive(false);
        //m_TurnEndButtonObj.GetComponent<Image>().enabled = true;
        //m_TurnEndButtonObj.GetComponentInChildren<Text>().enabled = true;
    }
}
