using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �o�^���ꂽ�֐����������s����N���X
public class WorkQueue
{
    // �V���O���g��
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


    public delegate bool Func(); //�߂�l��true�̂Ƃ��I������Ɣ��f
    Queue<Func> m_Funcs;
    public bool m_stop { get; private set; } = false;

    //�o�^�����֐���1���s
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

    //�߂�l��bool��true�ɂȂ�܂ŌJ��Ԃ��֐���o�^
    public void EnqueueLoopFunc(Func func, bool waitForAnimationEnd = true)
    {
        if (waitForAnimationEnd)
        {
            m_Funcs.Enqueue(AnimationQueue.Instance.IsAllAnimationEnd);
        }
        m_Funcs.Enqueue(func);
    }

    //��x�������s���Ȃ��֐���o�^
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

    //��x�������s���Ȃ��֐����܂Ƃ߂ēo�^
    public void EnqueueOnceRunFuncs(params Action[] actions)
    {
        foreach (Action action in actions)
        {
            EnqueueOnceRunFunc(action, true);
        }
    }

    // Stop �� Restart�͕K���Z�b�g�Ŏg��
    public void Stop()
    {
        m_stop = true;
    }
    public void Restart()
    {
        m_stop = false;
    }
}
