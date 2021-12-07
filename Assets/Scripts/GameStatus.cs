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


    public Queue<Mode> m_modeQueue;


    public PlayingPhase m_gamePhase { get; private set; }
    public bool m_isModeEnd { get; protected set; }
    public Turn m_turn { get; protected set; }

    public GameStatus()
    {
        m_modeQueue = new Queue<Mode>();
        m_isModeEnd = true;
    }

    public Mode GetNowMode()
    {
        if (m_modeQueue.Count == 0)
            return Mode.EMPTY;
        return m_modeQueue.Peek();
    }

    public void AddModeQueue(Mode mode)
    {
        if(mode == Mode.EMPTY)
        {
            Debug.LogError("You shouldn't set \"Mode.EMPTY\".");
        }
        m_modeQueue.Enqueue(mode);
    }

    public void ProceedModeQueue()
    {
        if (m_modeQueue.Count <= 1)
        {
            Debug.LogWarning("m_modeQueue doesn't have next MODE value.");
            return;
        }
        m_modeQueue.Dequeue();
    }

    public void StartMode()
    {
        m_isModeEnd = false;
    }
    public void FinishMode()
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
