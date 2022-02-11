using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

// GameManagerにstateパターンを適用
abstract class GameManagerState
{
    protected GameManagerState nextState = null;

    abstract public void Enter();
    abstract public GameManagerState Update();
    abstract public void Exit();
}

class PrepareCardState : GameManagerState
{
    Deck m_RootDeck;
    Deck m_MyDeck;
    Deck m_OppoDeck;
    Trush m_RightTrush;
    Trush m_LeftTrush;
    Hand m_MyHand;
    Hand m_OppoHand;
    UIManager m_UIManager;
    GameStatus m_gameStatus;

    Image m_ImageCardPrefab;

    public PrepareCardState(
        Deck rootDeck,
        Deck myDeck,
        Deck oppoDeck,
        Trush rightTrush,
        Trush leftTrush,
        Hand myHand,
        Hand oppoHand,
        UIManager uiManager,
        GameStatus gameStatus,
        
        Image imageCardPrefab)
    {
        m_RootDeck = rootDeck;
        m_MyDeck = myDeck;
        m_OppoDeck = oppoDeck;
        m_RightTrush = rightTrush;
        m_LeftTrush = leftTrush;
        m_MyHand = myHand;
        m_OppoHand = oppoHand;
        m_UIManager = uiManager;
        m_gameStatus = gameStatus;

        m_ImageCardPrefab = imageCardPrefab;
    }



    public override void Enter()
    {
        m_gameStatus.m_nowMode = GameStatus.Mode.PREPARE_CARD;
        Debug.Log("Gamemode: PREPARE_CARD");

        // やる処理をすべて登録しておく
        WorkQueue.Instance.EnqueueOnceRunFuncs(
                MakeAllCardsToRootDeck,
                m_RootDeck.Shuffle,
                MakeMyOppoDeck,
                MakeInitialHands,
                MakeInitialTrash,

                // TODO: どっちから始めるか
                m_gameStatus.SetTurnRandom
            );

        // 次のState登録
        WorkQueue.Instance.EnqueueOnceRunFunc(
            () =>
            {
                nextState = new PlayingState(
                    m_RootDeck, m_MyDeck, m_OppoDeck, m_RightTrush, m_LeftTrush, m_MyHand, m_OppoHand, m_UIManager, m_gameStatus);
            });
    }

    public override GameManagerState Update()
    {
        WorkQueue.Instance.RunFunc();
        return nextState;
    }
    public override void Exit()
    {
        nextState = null;
    }

// -------------------------------------------------------------------------------------------------------
    void MakeAllCardsToRootDeck()
    {
        Debug.Log("Making All Root Deck's Cards");


        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();



        for (Card.Suit s = Card.Suit.Club; s <= Card.Suit.Spade; s++)
        {
            for (int i = 1; i <= 13; i++)
            {
                Image newCardImageObj = Image.Instantiate(m_ImageCardPrefab);
                Card newCard = newCardImageObj.GetComponent<Card>();
                newCard.Initialize(newCardImageObj, s, i);
                newCard.name = s.ToString() + "_" + i.ToString();
                m_RootDeck.AddCard(newCard, false);
            }
        }
        Image joker1_imageObj = Image.Instantiate(m_ImageCardPrefab);
        Image joker2_imageObj = Image.Instantiate(m_ImageCardPrefab);
        Card joker1 = joker1_imageObj.GetComponent<Card>();
        Card joker2 = joker2_imageObj.GetComponent<Card>();
        joker1.Initialize(joker1_imageObj, Card.Suit.Joker, 1);
        joker2.Initialize(joker2_imageObj, Card.Suit.Joker, 2);
        joker1.name = "joker_1";
        joker2.name = "joker_2";
        m_RootDeck.AddCard(joker1, false);
        m_RootDeck.AddCard(joker2, false);

        m_RootDeck.SetViewOrder();
    }

    //配る
    void MakeMyOppoDeck()
    {
        Debug.Log("Making MyDeck and OppoDeck");

        int nLoop = m_RootDeck.m_cards.Count / 2;
        Vector3 myDeckPositon = m_MyDeck.transform.position;
        Vector3 oppoDeckPositon = m_OppoDeck.transform.position;
        var animList = new List<AnimationQueue.MethodAndWaitFrames>();

        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        for (int i = 0; i < nLoop; i++)
        {
            Card card1 = m_RootDeck.DrawCard();
            Card card2 = m_RootDeck.DrawCard();
            m_MyDeck.AddCard(card1, false);
            m_OppoDeck.AddCard(card2, false);

            // アニメーション処理
            IEnumerator<bool> animRetVal1 = card1.Anim_StraightLineMove(myDeckPositon);
            IEnumerator<bool> animRetVal2 = card2.Anim_StraightLineMove(oppoDeckPositon);
            int waitFrames = 10 + i;

            AnimationQueue.Instance.AddAnimToLastIndex(animRetVal1, waitFrames);
            AnimationQueue.Instance.AddAnimToLastIndex(animRetVal2, waitFrames);
        }
    }

    void MakeInitialHands()
    {
        Debug.Log("Making Initial Hands");
        for (int i = 0; i < Hand.INITIAL_CARDS_NUM; i++)
        {
            AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
            m_MyHand.AddCard(m_MyDeck.DrawCard());
            m_OppoHand.AddCard(m_OppoDeck.DrawCard());

            ////アニメーション処理
            //m_MyHand.DoAddCardAnim();
            //m_OppoHand.DoAddCardAnim();
        }
    }

    void MakeInitialTrash()
    {
        Debug.Log("Making Initial Trash");
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        m_RightTrush.AddCard(m_MyDeck.DrawCard());
        m_LeftTrush.AddCard(m_OppoDeck.DrawCard());

        ////アニメーション処理
        //m_RightTrush.DoDiscardAnim(true);
        //m_LeftTrush.DoDiscardAnim(true);


        //ジョーカーが出てしまった場合
        Deck dealDeck = m_MyDeck;
        Trush dealTrush = m_RightTrush;
        for (int i = 0; i < 2; ++i)
        {
            while (
                dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
            {
                AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                if (dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
                {
                    dealDeck.AddCard(dealTrush.DrawCard());

                    //TODO ジョーカーが一番上表示になってない?
                    //AnimationQueue.Instance.AddAnimToLastIndex(
                    //    dealDeck.m_cards[dealDeck.m_cards.Count - 1].Anim_StraightLineMoveWithTurnOver(dealDeck.transform.position), 60);
                    dealDeck.Shuffle();

                    //引き直し
                    AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                    dealTrush.AddCard(dealDeck.DrawCard());

                    ////アニメーション処理
                    //dealTrush.DoDiscardAnim(true);
                }
            }
            dealDeck = m_OppoDeck;
            dealTrush = m_LeftTrush;
        }
    }
}

class PlayingState : GameManagerState
{
    Deck m_RootDeck;
    Deck m_MyDeck;
    Deck m_OppoDeck;
    Trush m_RightTrush;
    Trush m_LeftTrush;
    Hand m_MyHand;
    Hand m_OppoHand;
    UIManager m_UIManager;
    GameStatus m_gameStatus;

    public PlayingState(
        Deck rootDeck, Deck myDeck, Deck oppoDeck, Trush rightTrush, Trush leftTrush, Hand myHand, Hand oppoHand, UIManager uiManager, GameStatus gameStatus)
    {
        m_RootDeck = rootDeck;
        m_MyDeck = myDeck;
        m_OppoDeck = oppoDeck;
        m_RightTrush = rightTrush;
        m_LeftTrush = leftTrush;
        m_MyHand = myHand;
        m_OppoHand = oppoHand;
        m_UIManager = uiManager;
        m_gameStatus = gameStatus;
    }
    //------------------------------------------------------------------------------------

    public override void Enter()
    {
        m_gameStatus.m_nowMode = GameStatus.Mode.PLAYING;
        Debug.Log("Gamemode: PLAYING");
        WorkQueue.Instance.EnqueueOnceRunFunc(StartTurn);
    }
    public override GameManagerState Update()
    {
        WorkQueue.Instance.RunFunc();
        return nextState;
    }
    public override void Exit()
    {
        throw new NotImplementedException();
    }

    // --------------------------------------------------------------------------------------
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
        } else
        {
            m_handlingDeck = m_OppoDeck;
            m_handlingHand = m_OppoHand;
            m_mainTrush = m_LeftTrush;
            m_subTrush = m_RightTrush;
        }
        WorkQueue.Instance.EnqueueOnceRunFunc(DrawFromDeck);
    }
    void DrawFromDeck()
    {
        Card topCard = m_handlingDeck.GetTopCard();
        Transform deckTransform = topCard.transform.parent.parent;

        topCard.m_canDrag = true;
        m_mainTrush.EnableReceiveDrop();
        if (m_handlingHand.CanAddCard())
        {
            m_handlingHand.EnableReceiveDrop();
        }
        WorkQueue.Instance.EnqueueLoopFunc(() =>
        {
            return topCard.transform.parent.parent != deckTransform;
        });
        WorkQueue.Instance.EnqueueOnceRunFuncs(() =>
        {
            topCard.m_canDrag = false;
            m_mainTrush.DisableReceiveDrop();
            m_handlingDeck.DisableReceiveDrop();
        });
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