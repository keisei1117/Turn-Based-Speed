using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 登録された関数を順次実行するクラス
public class UpdateFuncQueue
{
    public delegate bool UpdateFunc(); //戻り値がtrueのとき終わったと判断
    Queue<UpdateFunc> m_updateFuncQueue;

    public UpdateFuncQueue()
    {
        m_updateFuncQueue = new Queue<UpdateFunc>();
    }

    public void RunFunc()
    {
        if (m_updateFuncQueue.Count == 0) return;
        bool isEnd = m_updateFuncQueue.Peek()();
        if (isEnd)
        {
            m_updateFuncQueue.Dequeue();
        }
    }

    //戻り値がboolでtrueになるまで繰り返す関数を登録
    public void EnqueueLoopFunc(UpdateFunc func, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_updateFuncQueue.Enqueue(AnimationManager.Instance.IsAllAnimationEnd);
        }
        m_updateFuncQueue.Enqueue(func);
    }

    //一度しか実行しない関数を登録
    public void EnqueueOnceRunProcess(Action action, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_updateFuncQueue.Enqueue(AnimationManager.Instance.IsAllAnimationEnd);
        }
        m_updateFuncQueue.Enqueue(() =>
        {
            action();
            return true;
        });
    }

    //一度しか実行しない関数をまとめて登録
    public void EnqueueOnceRunProcesses(params Action[] actions)
    {
        foreach (Action action in actions)
        {
            EnqueueOnceRunProcess(action, true);
        }
    }
}
