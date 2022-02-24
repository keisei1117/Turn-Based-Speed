using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject DrawButtonObj;
    public GameObject DiscardButtonObj;
    public GameObject CombineButtonObj;
    public GameObject CompressButtonObj;
    public GameObject TurnEndButtonObj;
    public Button DrawButton;
    public Button DiscardButton;
    public Button CombineButton;
    public Button CompressButton;
    public Button TurnEndButton;



    public Text m_myOrOppoTurn;


    private void Awake()
    {
        DrawButton = DrawButtonObj.GetComponent<Button>();
        DiscardButton = DiscardButtonObj.GetComponent<Button>();
        CombineButton = CombineButtonObj.GetComponent<Button>();
        CompressButton = CompressButtonObj.GetComponent<Button>();
        TurnEndButton = TurnEndButtonObj.GetComponent<Button>();

        DrawButton.Disable();
        DiscardButton.Disable();
        CombineButton.Disable();
        CompressButton.Disable();
        TurnEndButton.Disable();
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

}
