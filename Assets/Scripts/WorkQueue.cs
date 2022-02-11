using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// “o˜^‚³‚ê‚½ŠÖ”‚ğ‡ŸÀs‚·‚éƒNƒ‰ƒX
public class WorkQueue
{
    // ƒVƒ“ƒOƒ‹ƒgƒ“
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


    public delegate bool Func(); //–ß‚è’l‚ªtrue‚Ì‚Æ‚«I‚í‚Á‚½‚Æ”»’f
    Queue<Func> m_Funcs;

    //“o˜^‚µ‚½ŠÖ”‚ğ1‚ÂÀs
    public void RunFunc()
    {
        if (m_Funcs.Count == 0) return;
        bool isEnd = m_Funcs.Peek()();
        if (isEnd)
        {
            m_Funcs.Dequeue();
        }
    }

    //–ß‚è’l‚ªbool‚Åtrue‚É‚È‚é‚Ü‚ÅŒJ‚è•Ô‚·ŠÖ”‚ğ“o˜^
    public void EnqueueLoopFunc(Func func, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_Funcs.Enqueue(AnimationQueue.Instance.IsAllAnimationEnd);
        }
        m_Funcs.Enqueue(func);
    }

    //ˆê“x‚µ‚©Às‚µ‚È‚¢ŠÖ”‚ğ“o˜^
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
    }

    //ˆê“x‚µ‚©Às‚µ‚È‚¢ŠÖ”‚ğ‚Ü‚Æ‚ß‚Ä“o˜^
    public void EnqueueOnceRunFuncs(params Action[] actions)
    {
        foreach (Action action in actions)
        {
            EnqueueOnceRunFunc(action, true);
        }
    }
}
