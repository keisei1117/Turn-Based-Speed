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

    public GameStatus m_gameStatus { get; protected set; }

    const GameStatus.Mode INITIAL_MODE = GameStatus.Mode.PREPARE_CARD;
    bool m_isFirstUpdate;

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
                m_isFirstUpdate = false;
            }
            else
            {
                //次のモードへ
                m_gameStatus.ProceedModeQueue();
            }
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

    private void FixedUpdate()
    {
        AnimationManager.Instance.DoAnimation();
    }

    IEnumerator Mode_PrepareCards()
    {
        Debug.Log("Gamemode: PREPARE_CARD");

        yield return new WaitForSeconds(1);

        MakeAllCardsToRootDeck();
        m_RootDeck.Shuffle();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());
        MakeMyOppoDeck();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());
        MakeInitialHands();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());
        MakeInitialTrash();
        yield return StartCoroutine(AnimationManager.Instance.Coroutine_WaitAllAnimEnd());

        // TODO: どっちから始めるか
        m_gameStatus.SetTurnRandom();

        //↓必須処理
        m_gameStatus.FinishMode();
        yield break;
    } 

    IEnumerator Mode_Playing()
    {
        switch (m_gameStatus.m_gamePhase)
        {
            case GameStatus.PlayingPhase.TURN_START:
                break;
            case GameStatus.PlayingPhase.DRAW:
                break;
            case GameStatus.PlayingPhase.OPERATE:
                break;
            case GameStatus.PlayingPhase.SERVE:
                break;
            case GameStatus.PlayingPhase.TURN_END:
                break;
        }

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
                m_RootDeck.AddCard(newCard);
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
        m_RootDeck.AddCard(joker1);
        m_RootDeck.AddCard(joker2);
    }

    //配る
    void MakeMyOppoDeck()
    {
        Debug.Log("Making MyDeck and OppoDeck");

        int nLoop = m_RootDeck.m_cards.Count / 2;
        Vector3 myDeckPositon = m_MyDeck.transform.position;
        Vector3 oppoDeckPositon = m_OppoDeck.transform.position;
        var animList = new List<AnimationManager.MethodAndWaitFrames>();

        AnimationManager.Instance.CreateNewEmptyAnimListToEnd();
        for (int i = 0; i < nLoop; i++)
        {
            Card card1 = m_RootDeck.DrawCard();
            Card card2 = m_RootDeck.DrawCard();
            m_MyDeck.AddCard(card1);
            m_OppoDeck.AddCard(card2);

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
            m_MyHand.AddCard(m_MyDeck.DrawCard());
            m_OppoHand.AddCard(m_OppoDeck.DrawCard());

            //アニメーション処理
            AnimationManager.Instance.CreateNewEmptyAnimListToEnd();
            m_MyHand.DoAddCardAnim();
            m_OppoHand.DoAddCardAnim();
        }
    }

    void MakeInitialTrash()
    {
        m_RightTrush.AddCard(m_MyDeck.DrawCard());
        m_LeftTrush.AddCard(m_OppoDeck.DrawCard());

        //アニメーション処理
        AnimationManager.Instance.CreateNewEmptyAnimListToEnd();
        m_RightTrush.DoDiscardAnim(true);
        m_LeftTrush.DoDiscardAnim(true);


        //ジョーカーが出てしまった場合
        Deck dealDeck = m_MyDeck;
        Trush dealTrush = m_RightTrush;
        for (int i = 0; i < 2; ++i)
        {
            while (
                dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
            {
                AnimationManager.Instance.CreateNewEmptyAnimListToEnd();
                if (dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
                {
                    dealDeck.AddCard(dealTrush.DrawCard());

                    //TODO ジョーカーが一番上表示になってない?
                    AnimationManager.Instance.AddAnimToLastIndex(
                        dealDeck.m_cards[dealDeck.m_cards.Count - 1].Anim_StraightLineMoveWithTurnOver(dealDeck.transform.position), 60);
                    dealDeck.Shuffle();

                    dealTrush.AddCard(dealDeck.DrawCard());

                    //アニメーション処理
                    AnimationManager.Instance.CreateNewEmptyAnimListToEnd();
                    dealTrush.DoDiscardAnim(true);
                }
            }
            dealDeck = m_OppoDeck;
            dealTrush = m_LeftTrush;
        }
    }
}
