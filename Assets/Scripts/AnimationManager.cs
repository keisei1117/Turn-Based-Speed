using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager
{
    // シングルトン実装
    private static AnimationManager instance;
    public static AnimationManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new AnimationManager();
                if (null == instance)
                {
                    Debug.Log(" AnimationManager Instance Error ");
                }
            }
            return instance;
        }
    }

    // アニメーションを行うメソッドの戻り値(IEnumerator<bool>)と待ちフレーム数をセットで保持する
    public class MethodAndWaitFrames
    {
        public IEnumerator<bool> method;
        // 待ちフレーム数
        public int waitFrames;

        public MethodAndWaitFrames(IEnumerator<bool> method, int waitFrames = 0)
        {
            this.method = method;
            this.waitFrames = waitFrames;
        }
    }

    List<List<MethodAndWaitFrames>> m_animMethods;

    public AnimationManager()
    {
        m_animMethods = new List<List<MethodAndWaitFrames>>();
    }

    public void DoAnimation()
    {
        if (m_animMethods.Count == 0) return;

        for (int i = 0; i < m_animMethods[0].Count; i++)
        {
            bool shouldWait = m_animMethods[0][i].waitFrames != 0;
            if (shouldWait)
            {
                m_animMethods[0][i].waitFrames--;
            }
            else
            {
                m_animMethods[0][i].method.MoveNext();
            }

            bool isEnd = m_animMethods[0][i].method.Current;
            if (isEnd)
            {
                m_animMethods[0].RemoveAt(i);
                Debug.Log("animMethods remove");
            }
        }

        bool isEmptyList = m_animMethods[0].Count == 0;
        if (isEmptyList)
        {
            m_animMethods.RemoveAt(0);
        }
    }

    public void AddAnimToFirstIndex(IEnumerator<bool> retValOfAnimMethod, int waitFrames = 0)
    {
        Debug.Log("AddPlayingAnimation");
        MethodAndWaitFrames additionalMethod = new MethodAndWaitFrames(retValOfAnimMethod, waitFrames);
        if (m_animMethods.Count == 0)
            m_animMethods.Add(new List<MethodAndWaitFrames>());
        m_animMethods[0].Add(additionalMethod);
    }
    public void AddAnimToLastIndex(IEnumerator<bool> retValOfAnimMethod, int waitFrames = 0)
    {
        Debug.Log("AddAnimToLastIndex");
        MethodAndWaitFrames additionalMethod = new MethodAndWaitFrames(retValOfAnimMethod, waitFrames);
        m_animMethods[m_animMethods.Count - 1].Add(additionalMethod);
    }
    public void CreateNewEmptyAnimListToEnd()
    {
        Debug.Log("CreateNewEmptyAnimListToEnd");
        m_animMethods.Add(new List<MethodAndWaitFrames>());
    }

    public bool IsAllAnimationEnd()
    {
        Debug.Log("m_animMethods.Count: " + m_animMethods.Count);
        if (m_animMethods.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator Coroutine_WaitAllAnimEnd()
    {
        while (!IsAllAnimationEnd())
        {
            yield return null;
        }
        Debug.Log("All Animations end.");
    }
}
