using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// “o˜^‚³‚ê‚½ŠÖ”‚ğ‡ŸÀs‚·‚éƒNƒ‰ƒX
public class UpdateFuncQueue
{
    public delegate bool UpdateFunc(); //–ß‚è’l‚ªtrue‚Ì‚Æ‚«I‚í‚Á‚½‚Æ”»’f
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

    //–ß‚è’l‚ªbool‚Åtrue‚É‚È‚é‚Ü‚ÅŒJ‚è•Ô‚·ŠÖ”‚ğ“o˜^
    public void EnqueueLoopFunc(UpdateFunc func, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_updateFuncQueue.Enqueue(AnimationManager.Instance.IsAllAnimationEnd);
        }
        m_updateFuncQueue.Enqueue(func);
    }

    //ˆê“x‚µ‚©Às‚µ‚È‚¢ŠÖ”‚ğ“o˜^
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

    //ˆê“x‚µ‚©Às‚µ‚È‚¢ŠÖ”‚ğ‚Ü‚Æ‚ß‚Ä“o˜^
    public void EnqueueOnceRunProcesses(params Action[] actions)
    {
        foreach (Action action in actions)
        {
            EnqueueOnceRunProcess(action, true);
        }
    }
}
