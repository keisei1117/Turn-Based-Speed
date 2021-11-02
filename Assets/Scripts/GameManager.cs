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
                //Debug.Log("Gamemode: START");
                //MakeAllCardsToRootDeck();
                //m_RootDeck.GetComponent<Deck>().Shuffle();
                //MakeMyOppoDeck();
                StartCoroutine(Mode_Start());

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
    IEnumerator Mode_Start()
    {
        Debug.Log("Gamemode: START");
        MakeAllCardsToRootDeck();
        m_RootDeck.GetComponent<Deck>().Shuffle();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());

        MakeMyOppoDeck();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());
    }

    void MakeAllCardsToRootDeck()
    {
        Debug.Log("Making All Root Deck's Cards");

        for (Card.Suit s = Card.Suit.Club; s <= Card.Suit.Spade; s++)
        {
            for (int i = 1; i <= 13; i++)
            {
                Image newCardImageObj = Instantiate(m_ImageCardPrefab);
                Card newCard = newCardImageObj.GetComponent<Card>();
                newCard.Initialize(newCardImageObj, s, i);
                newCard.name = s.ToString() + "_" + i.ToString();
                m_RootDeck.GetComponent<Deck>().AddCard(newCard);
            }
        }
        Image joker1_imageObj = Instantiate(m_ImageCardPrefab);
        Image joker2_imageObj = Instantiate(m_ImageCardPrefab);
        Card joker1 = joker1_imageObj.GetComponent<Card>();
        Card joker2 = joker2_imageObj.GetComponent<Card>();
        joker1.Initialize(joker1_imageObj, Card.Suit.Joker, 1);
        joker2.Initialize(joker2_imageObj, Card.Suit.Joker, 2);
        joker1.name = "joker_1";
        joker2.name = "joker_2";
        m_RootDeck.GetComponent<Deck>().AddCard(joker1);
        m_RootDeck.GetComponent<Deck>().AddCard(joker2);
    }

    //配る
    public void MakeMyOppoDeck()
    {
        Debug.Log("Making MyDeck and OppoDeck");

        int nLoop = m_RootDeck.GetComponent<Deck>().m_cards.Count / 2;
        Vector3 myDeckPositon = m_MyDeck.transform.position;
        Vector3 oppoDeckPositon = m_OppoDeck.transform.position;
        var animList = new List<AnimationManager.MethodAndWaitFrames>();

        for (int i = 0; i < nLoop; i++)
        {
            Card card1 = m_RootDeck.GetComponent<Deck>().DrawCard();
            Card card2 = m_RootDeck.GetComponent<Deck>().DrawCard();
            m_MyDeck.GetComponent<Deck>().AddCard(card1);
            m_OppoDeck.GetComponent<Deck>().AddCard(card2);

            // アニメーション処理
            IEnumerator<bool> animRetVal1 = card1.Anim_StraightLineMove(myDeckPositon);
            IEnumerator<bool> animRetVal2 = card2.Anim_StraightLineMove(oppoDeckPositon);
            int waitFrames =  10 + i;
            animList.Add(new AnimationManager.MethodAndWaitFrames(animRetVal1, waitFrames));
            animList.Add(new AnimationManager.MethodAndWaitFrames(animRetVal2, waitFrames));
        }
        AnimationManager.Instance.AddPlayingAnimationList(animList, true);
    }
}
