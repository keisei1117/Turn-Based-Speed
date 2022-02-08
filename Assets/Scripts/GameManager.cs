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

    UpdateFuncQueue m_updateFuncQueue;


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
        m_updateFuncQueue = new UpdateFuncQueue();
    }

    // Start is called before the first frame updat e
    void Start()
    {
        Card.LoadImages();
        m_isFirstUpdate = true;
        m_updateFuncQueue.EnqueueOnceRunProcess(Mode_PrepareCards);
    }

    // Update is called once per frame
    void Update()
    {
        m_updateFuncQueue.RunFunc();
    }

    private void FixedUpdate()
    {
        AnimationManager.Instance.DoAnimation();
    }

    void Mode_PrepareCards()
    {
        m_gameStatus.m_nowMode = GameStatus.Mode.PREPARE_CARD;
        Debug.Log("Gamemode: PREPARE_CARD");

        m_updateFuncQueue.EnqueueOnceRunProcesses(
                MakeAllCardsToRootDeck,
                m_RootDeck.Shuffle,
                MakeMyOppoDeck,
                MakeInitialHands,
                MakeInitialTrash,

                // TODO: どっちから始めるか
                m_gameStatus.SetTurnRandom
            );

        m_updateFuncQueue.EnqueueOnceRunProcess(Mode_Playing);
    }

    void Mode_Playing()
    {
        m_gameStatus.m_nowMode = GameStatus.Mode.PLAYING;
        Debug.Log("Gamemode: PLAYING");

        m_updateFuncQueue.EnqueueOnceRunProcess( StartTurn );
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

    Deck m_handlingDeck;
    Hand m_handlingHand;
    Trush m_mainTrush, m_subTrush;
    void StartTurn()
    {
        if (m_gameStatus.m_turn == GameStatus.Turn.MY_TURN)
        {
            m_handlingDeck = m_MyDeck;
            m_handlingHand = m_MyHand;
            m_mainTrush = m_RightTrush;
            m_subTrush = m_LeftTrush;
        }
        //TURN_START:
        m_updateFuncQueue.EnqueueLoopFunc( DrawFromDeck );
    }
    bool DrawFromDeck() {
        return false;
        //bool isCardDraged =
        //    m_handlingDeck.m_cards[0].IsDraggedOn(m_handlingHand.m_rangeObj) ||
        //    m_handlingDeck.m_cards[0].IsDraggedOn(m_mainTrush.m_rangeObj);
    }
    bool OpperateCardPhase()
    {
        return true;

        //TODO: ゲーム終了判定
        bool isGameEnd = false;
        if (isGameEnd) return true;
    }
    void TurnEnd() { }
}
