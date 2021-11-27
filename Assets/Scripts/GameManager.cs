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
    public GameObject m_UIManager;
    public Image m_ImageCardPrefab;

    public GameStatus m_gameStatus { get; protected set; }

    const GameStatus.Mode INITIAL_MODE = GameStatus.Mode.PREPARE_CARD;
    bool m_isFirstUpdate;

    private void Awake()
    {
        // TODO: 確認不十分　いちいち書くのめんどい
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

        m_gameStatus = new GameStatus();
        m_gameStatus.AddModeQueue(INITIAL_MODE);
    }

    // Start is called before the first frame updat e
    void Start()
    {
        Card.LoadImages();
        m_isFirstUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_gameStatus.m_isModeEnd)
        {
            if (m_isFirstUpdate)
            {
                m_isFirstUpdate=false;
            }
            //次のモードへ
            m_gameStatus.ProceedModeQueue();
        }
        else
        {
            return; //コルーチン処理待ち
        }

        Debug.Log("nowMode: " + m_gameStatus.GetNowMode());
        m_gameStatus.StartMode();
        switch (m_gameStatus.GetNowMode())
        {
            case GameStatus.Mode.PREPARE_CARD:
                StartCoroutine(Mode_PrepareCards());
                m_gameStatus.AddModeQueue(GameStatus.Mode.PLAYING);
                break;
            case GameStatus.Mode.PLAYING:
                break;
        }
    }

    IEnumerator Mode_PrepareCards()
    {
        Debug.Log("Gamemode: PREPARE_CARD");
        MakeAllCardsToRootDeck();
        m_RootDeck.GetComponent<Deck>().Shuffle();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());

        MakeMyOppoDeck();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());

        // TODO: どっちから始めるか
        m_gameStatus.SetTurnRandom();

        // TODO: カードを引く
        MakeInitialHands();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());

        //↓必須処理
        m_gameStatus.FinishMode();
        yield break;
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
    void MakeMyOppoDeck()
    {
        Debug.Log("Making MyDeck and OppoDeck");

        int nLoop = m_RootDeck.GetComponent<Deck>().m_cards.Count / 2;
        Vector3 myDeckPositon = m_MyDeck.transform.position;
        Vector3 oppoDeckPositon = m_OppoDeck.transform.position;
        var animList = new List<AnimationManager.MethodAndWaitFrames>();

        AnimationManager.Instance.CreateNewEmptyAnimListToEnd();
        for (int i = 0; i < nLoop; i++)
        {
            Card card1 = m_RootDeck.GetComponent<Deck>().DrawCard();
            Card card2 = m_RootDeck.GetComponent<Deck>().DrawCard();
            m_MyDeck.GetComponent<Deck>().AddCard(card1);
            m_OppoDeck.GetComponent<Deck>().AddCard(card2);

            // アニメーション処理をAnimationManagerに渡す
            IEnumerator<bool> animRetVal1 = card1.Anim_StraightLineMove(myDeckPositon);
            IEnumerator<bool> animRetVal2 = card2.Anim_StraightLineMove(oppoDeckPositon);
            int waitFrames = 10 + i;

            AnimationManager.Instance.AddAnimToLastIndex(animRetVal1, waitFrames);
            AnimationManager.Instance.AddAnimToLastIndex(animRetVal2, waitFrames);
        }
    }

    void MakeInitialHands()
    {
        for(int i = 0; i < Hand.INITIAL_CARDS_NUM; i++)
        {
            m_MyHand.GetComponent<Hand>().AddCard(m_MyDeck.GetComponent<Deck>().DrawCard(), true);
            m_OppoHand.GetComponent<Hand>().AddCard(m_OppoDeck.GetComponent<Deck>().DrawCard(), false);
        }
    }
}
