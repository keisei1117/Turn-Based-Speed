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
}
