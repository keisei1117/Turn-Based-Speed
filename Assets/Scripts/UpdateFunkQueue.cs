using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �o�^���ꂽ�֐����������s����N���X
public class UpdateFuncQueue
{
    public delegate bool UpdateFunc(); //�߂�l��true�̂Ƃ��I������Ɣ��f
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

    //�߂�l��bool��true�ɂȂ�܂ŌJ��Ԃ��֐���o�^
    public void EnqueueLoopFunc(UpdateFunc func, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_updateFuncQueue.Enqueue(AnimationManager.Instance.IsAllAnimationEnd);
        }
        m_updateFuncQueue.Enqueue(func);
    }

    //��x�������s���Ȃ��֐���o�^
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

    //��x�������s���Ȃ��֐����܂Ƃ߂ēo�^
    public void EnqueueOnceRunProcesses(params Action[] actions)
    {
        foreach (Action action in actions)
        {
            EnqueueOnceRunProcess(action, true);
        }
    }
}
