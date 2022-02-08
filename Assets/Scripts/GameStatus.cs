using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus
{
    public enum Mode
    {
        STANDBY,
        PREPARE_CARD,
        PLAYING,
        POUSE,
        EMPTY,
    }
    public enum PlayingPhase
    {
        TURN_START,
        DRAW,
        OPERATE,
        SERVE,
        TURN_END,
    }
    public enum Turn
    {
        MY_TURN,
        OPPO_TURN,
    }


    public Mode m_nowMode { get; set; }
    public PlayingPhase m_gamePhase { get; private set; }
    public bool m_isModeEnd { get; protected set; }
    public Turn m_turn { get; protected set; }

    public GameStatus()
    {
        m_isModeEnd = true;
    }

    public void SetTurnRandom()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        int zeroOrOne = (int)(0.5f + UnityEngine.Random.value);
        bool isMyTurn = (zeroOrOne == 0);
        if (isMyTurn)
        {
            m_turn = Turn.MY_TURN;
        }
        else
        {
            m_turn = Turn.OPPO_TURN;
        }
        Debug.Log("Result of SetTurnRandom: m_turn == " + m_turn.ToString());
    }

    public bool IsMyTurn()
    {
        return m_turn == Turn.MY_TURN;
    }
    public bool IsOppoTurn()
    {
        return m_turn == Turn.OPPO_TURN;
    }
    public void SetTurnToMyTurn()
    {
        m_turn = Turn.MY_TURN;
    }
    public void SetTurnToOppoTurn()
    {
        m_turn = Turn.OPPO_TURN;
    }
    public void SwitchTurn()
    {
        if (m_turn == Turn.MY_TURN)
        {
            m_turn = Turn.OPPO_TURN;
        }
        else
        {
            m_turn = Turn.MY_TURN;
        }
    }

    public void ProceedGamePhase()
    {
        if (m_gamePhase == PlayingPhase.TURN_END)
        {
            m_gamePhase = PlayingPhase.TURN_START;
            SwitchTurn();
            return;
        }
        m_gamePhase++;
    }
}
