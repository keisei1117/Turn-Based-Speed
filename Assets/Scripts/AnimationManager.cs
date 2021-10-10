using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
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
                    Debug.Log(" DataManager Instance Error ");
                }
            }
            return instance;
        }
    }


    public delegate IEnumerator<bool> AnimMethod();
    static List<List<IEnumerator<bool>>> m_animMethods;

    private void Awake()
    {
        m_animMethods = new List<List<IEnumerator<bool>>>();
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
            m_animMethods[0][i].MoveNext();

            if (m_animMethods[0][i].Current)
            {
                m_animMethods[0].RemoveAt(i);
                Debug.Log("animMethods remove");
            }
        }
        if (m_animMethods[0].Count == 0)
        {
            m_animMethods.RemoveAt(0);
        }
    }

    public void AddPlayingAnimation(AnimMethod method, bool ifWait)
    {
        Debug.Log("AddPlayingAnimation");
        if (ifWait)
        {
            m_animMethods.Add(new List<IEnumerator<bool>>());
            m_animMethods[m_animMethods.Count-1].Add(method());
        }
        else
        {
            m_animMethods[0].Add(method());
        }
    }

    public bool IfAnimationPlaying()
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
