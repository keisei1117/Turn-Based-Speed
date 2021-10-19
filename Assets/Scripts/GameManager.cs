using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class GameManager : MonoBehaviour
{
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


public GameObject m_RootDeck;
    public GameObject m_MyDeck;
    public GameObject m_OppoDeck;
    public GameObject m_RightTrush;
    public GameObject m_LeftTrush;
    public GameObject m_MyHand;
    public GameObject m_OppoHand;

    public GameObject m_AnimatinoManager;

    public Image m_ImageCardPrefab;

    enum GameMode
    {
        STANDBY,
        START,
        PLAYING,
        POUSE,
        WAITING_ANIMATION,
    }
    GameMode m_nowMode, m_preMode;

    private void Awake()
    {
        if (m_RootDeck == null ||
            m_MyDeck == null ||
            m_OppoDeck == null ||
            m_RightTrush == null ||
            m_LeftTrush == null ||
            m_MyHand == null ||
            m_OppoDeck == null)
        {
            throw new NullReferenceException("Game Object instance is null.");
        }

        m_nowMode = GameMode.START;
        m_preMode = GameMode.STANDBY;
    }

    // Start is called before the first frame update
    void Start()
    {
        Card.LoadImages();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_nowMode)
        {
            case GameMode.START:
                Debug.Log("Gamemode: START");
                MakeAllCardsToRootDeck();
                m_RootDeck.GetComponent<Deck>().Shuffle();
                MakeMyOppoDeck();

                m_nowMode = GameMode.WAITING_ANIMATION;
                break;

            case GameMode.WAITING_ANIMATION:
                Debug.Log("Gamemode: WAITING_ANIMATION");
                if (!m_AnimatinoManager.GetComponent<AnimationManager>().IsAllAnimationEnd())
                {
                    m_nowMode = m_preMode;
                }
                break;
        }
    }

    // TODO:    各ゲームモードのプロセスをコルーチン内に入れる。
    //          アニメーションが終わるのをまつコルーチンを作る
    //IEnumerator Mode_Start()
    //{

    //}

    void MakeAllCardsToRootDeck()
    {
        Debug.Log("Making All Root Deck's Cards");

        for (Card.Suit s = Card.Suit.Club; s <= Card.Suit.Spade; s++)
        {
            for (int i = 1; i <= 13; i++)
            {
                m_RootDeck.GetComponent<Deck>().AddCard(new Card(s, i, false));
            }
        }
        m_RootDeck.GetComponent<Deck>().AddCard(new Card(Card.Suit.Joker, 1, false));
        m_RootDeck.GetComponent<Deck>().AddCard(new Card(Card.Suit.Joker, 2, false));
    }

    public void MakeMyOppoDeck()
    {
        Debug.Log("Making MyDeck and OppoDeck");

        int nLoop = m_RootDeck.GetComponent<Deck>().m_cards.Count / 2;
        Vector3 rootDeckPositon = m_RootDeck.transform.Find("Canvas/ImageCard").position;
        Vector3 myDeckPositon = m_MyDeck.transform.Find("Canvas/ImageCard").position;
        Vector3 oppoDeckPositon = m_OppoDeck.transform.Find("Canvas/ImageCard").position;
        Transform rootDeckCanvasTransform = m_RootDeck.transform.Find("Canvas").transform;
        var animList = new List<AnimationManager.MethodAndWaitFrames>();

        for (int i = 0; i < nLoop; i++)
        {
            Card card1 = m_RootDeck.GetComponent<Deck>().DrawCard();
            Card card2 = m_RootDeck.GetComponent<Deck>().DrawCard();
            m_MyDeck.GetComponent<Deck>().AddCard(card1);
            m_OppoDeck.GetComponent<Deck>().AddCard(card2);

            IEnumerator<bool> animRetVal1 = Card.Anim_StraightLineMove(card1.GetImage(), rootDeckPositon, myDeckPositon, rootDeckCanvasTransform);
            IEnumerator<bool> animRetVal2 = Card.Anim_StraightLineMove(card2.GetImage(), rootDeckPositon, oppoDeckPositon, rootDeckCanvasTransform);

            int waitFrames =  10 + i;
            animList.Add(new AnimationManager.MethodAndWaitFrames(animRetVal1, waitFrames));
            animList.Add(new AnimationManager.MethodAndWaitFrames(animRetVal2, waitFrames));
        }
        AnimationManager.Instance.AddPlayingAnimationList(animList, true);
    }
}
