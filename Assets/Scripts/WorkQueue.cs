using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 登録された関数を順次実行するクラス
public class WorkQueue
{
    // シングルトン
    static private WorkQueue m_instance;
    public static WorkQueue Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new WorkQueue();
            }
            return m_instance;
        }
    }
    private WorkQueue()
    {
        m_Funcs = new Queue<Func>();
    }


    public delegate bool Func(); //戻り値がtrueのとき終わったと判断
    Queue<Func> m_Funcs;
    public bool m_stop { get; private set; } = false;

    //登録した関数を1つ実行
    public void RunFunc()
    {
        if (m_stop) return;
        if (m_Funcs.Count == 0) return;
        bool isEnd = m_Funcs.Peek()();
        if (isEnd)
        {
            m_Funcs.Dequeue();
        }
    }

    //戻り値がboolでtrueになるまで繰り返す関数を登録
    public void EnqueueLoopFunc(Func func, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_Funcs.Enqueue(AnimationQueue.Instance.IsAllAnimationEnd);
        }
        m_Funcs.Enqueue(func);
    }

    //一度しか実行しない関数を登録
    public void EnqueueOnceRunFunc(Action action, bool waitForAnimationEnd = true)
    {

        if (waitForAnimationEnd)
        {
            m_Funcs.Enqueue(AnimationQueue.Instance.IsAllAnimationEnd);
        }
        m_Funcs.Enqueue(() =>
        {
            action();
            return true;
        });

        if (m_Funcs.Count > 100)
        {
            Debug.LogError("Count of m_Funcs is more than 100.");
        }
    }

    //一度しか実行しない関数をまとめて登録
    public void EnqueueOnceRunFuncs(params Action[] actions)
    {
        foreach (Action action in actions)
        {
            EnqueueOnceRunFunc(action, true);
        }
    }

    // Stop と Restartは必ずセットで使う
    public void Stop()
    {
        m_stop = true;
    }
    public void Restart()
    {
        m_stop = false;
    }
}
