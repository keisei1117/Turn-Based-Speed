using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class GameManager : MonoBehaviour
{
    //シングルトン実装
    private static GameManager instance;
    public static GameManager Instance {
        get
        {
            if(instance == null)
            {
                instance = (GameManager)FindObjectOfType(typeof(GameManager));
                if(instance == null)
                {
                    Debug.Log(" GameManager Instance Error ");
                }
            }
            return instance;
        }
    }
    //-------------------------------------------------

    public GameObject m_RootDeckObj;
    public GameObject m_MyDeckObj;
    public GameObject m_OppoDeckObj;
    public GameObject m_RightTrushObj;
    public GameObject m_LeftTrushObj;
    public GameObject m_MyHandObj;
    public GameObject m_OppoHandObj;
    public GameObject m_UIManagerObj;
    Deck m_RootDeck;
    Deck m_MyDeck;
    Deck m_OppoDeck;
    Trush m_RightTrush;
    Trush m_LeftTrush;
    Hand m_MyHand;
    Hand m_OppoHand;
    UIManager m_UIManager;

    public Image m_ImageCardPrefab;
    public Canvas m_TopLayerCanvas;

    //private GameStatus m_gameStatus { get; protected set; }
    public GameStatus m_gameStatus;

    //WorkQueue WorkQueue.Instance;

    GameManagerState m_nowState;

    private void Awake()
    {
        // TODO: 確認不十分　いちいち書くのめんどい
        if (m_RootDeckObj == null ||
            m_MyDeckObj == null ||
            m_OppoDeckObj == null ||
            m_RightTrushObj == null ||
            m_LeftTrushObj == null ||
            m_MyHandObj == null ||
            m_OppoDeckObj == null)
        {
            throw new NullReferenceException("Game Object instance is null.");
        }

        m_RootDeck = m_RootDeckObj.GetComponent<Deck>();
        m_MyDeck = m_MyDeckObj.GetComponent<Deck>();
        m_OppoDeck = m_OppoDeckObj.GetComponent<Deck>();
        m_RightTrush = m_RightTrushObj.GetComponent<Trush>();
        m_LeftTrush = m_LeftTrushObj.GetComponent<Trush>();
        m_MyHand = m_MyHandObj.GetComponent<Hand>();
        m_OppoHand = m_OppoHandObj.GetComponent<Hand>();
        m_UIManager = m_UIManagerObj.GetComponent<UIManager>();

        m_gameStatus = new GameStatus();
    }

    // Start is called before the first frame updat e
    void Start()
    {
        Card.LoadImages();
        m_nowState = new PrepareCardState(
            m_RootDeck, m_MyDeck, m_OppoDeck, m_RightTrush, m_LeftTrush, m_MyHand, m_OppoHand, m_UIManager, m_gameStatus,
            m_ImageCardPrefab);
        m_nowState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        GameManagerState nextState;
        nextState = m_nowState.Update();
        if (nextState != null)
        {
            m_nowState.Exit();
            m_nowState = nextState;
            m_nowState.Enter();
            nextState = null;
        }
    }

    private void FixedUpdate()
    {
        AnimationQueue.Instance.DoAnimation();

        //3倍速
        //AnimationQueue.Instance.DoAnimation();
        //AnimationQueue.Instance.DoAnimation();
        //AnimationQueue.Instance.DoAnimation();
    }
}
