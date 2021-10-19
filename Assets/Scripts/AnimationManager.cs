using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // シングルトン実装
    private static AnimationManager instance;
    public static AnimationManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (AnimationManager)FindObjectOfType(typeof(AnimationManager));
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

        public MethodAndWaitFrames(IEnumerator<bool> method, int waitFrames)
        {
            this.method = method;
            this.waitFrames = waitFrames;
        }
    }

    static List<List<MethodAndWaitFrames>> m_animMethods;

    private void Awake()
    {
        m_animMethods = new List<List<MethodAndWaitFrames>>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        DoAnimation();
    }

    void DoAnimation()
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

    public void AddPlayingAnimation(IEnumerator<bool> retValOfAnimMethod, bool waitFormerAnimation, int waitFrames = 0)
    {
        Debug.Log("AddPlayingAnimation");
        MethodAndWaitFrames additionalMethod = new MethodAndWaitFrames(retValOfAnimMethod, waitFrames);
        if (waitFormerAnimation)
        {
            m_animMethods.Add(new List<MethodAndWaitFrames>());
            m_animMethods[m_animMethods.Count - 1].Add(additionalMethod);
        }
        else
        {
            m_animMethods[0].Add(additionalMethod);
        }
    }

    public void AddPlayingAnimationList(List<MethodAndWaitFrames> animList, bool waitFormerAnimation)
    {
        Debug.Log("AddPlayingAnimationList");
        if (waitFormerAnimation)
        {
            m_animMethods.Add(new List<MethodAndWaitFrames>());
            m_animMethods[m_animMethods.Count - 1].AddRange(animList);

        }
        else
        {
            m_animMethods[0].AddRange(animList);
        }
    }

    public bool IsAllAnimationEnd()
    {
        if (m_animMethods.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
