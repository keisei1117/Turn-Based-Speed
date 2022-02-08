using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

// GameManagerにstateパターンを適用
abstract class GameManagerState
{
    abstract public void Enter();
    abstract public void Update();
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
        ref Deck rootDeck,
        ref Deck myDeck,
        ref Deck oppoDeck,
        ref Trush rightTrush,
        ref Trush leftTrush,
        ref Hand myHand,
        ref Hand oppoHand,
        ref UIManager uiManager,
        ref GameStatus gameStatus,
        
        ref Image imageCardPrefab)
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

        WorkQueue.Instance.EnqueueOnceRunProcesses(
                MakeAllCardsToRootDeck,
                m_RootDeck.Shuffle,
                MakeMyOppoDeck,
                MakeInitialHands,
                MakeInitialTrash,

                // TODO: どっちから始めるか
                m_gameStatus.SetTurnRandom
            );
        //WorkQueue.Instance.EnqueueOnceRunProcess(Mode_Playing);
    }
    public override void Update()
    {
        WorkQueue.Instance.RunFunc();
    }
    public override void Exit()
    {
    }

// -------------------------------------------------------------------------------------------------------
    void MakeAllCardsToRootDeck()
    {
        Debug.Log("Making All Root Deck's Cards");

        for (Card.Suit s = Card.Suit.Club; s <= Card.Suit.Spade; s++)
        {
            for (int i = 1; i <= 13; i++)
            {
                Image newCardImageObj = Image.Instantiate(m_ImageCardPrefab);
                Card newCard = newCardImageObj.GetComponent<Card>();
                newCard.Initialize(newCardImageObj, s, i);
                newCard.name = s.ToString() + "_" + i.ToString();
                m_RootDeck.AddCard(newCard);
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
        var animList = new List<AnimationQueue.MethodAndWaitFrames>();

        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        for (int i = 0; i < nLoop; i++)
        {
            Card card1 = m_RootDeck.DrawCard();
            Card card2 = m_RootDeck.DrawCard();
            m_MyDeck.AddCard(card1);
            m_OppoDeck.AddCard(card2);

            // アニメーション処理をAnimationQueueに渡す
            IEnumerator<bool> animRetVal1 = card1.Anim_StraightLineMove(myDeckPositon);
            IEnumerator<bool> animRetVal2 = card2.Anim_StraightLineMove(oppoDeckPositon);
            int waitFrames = 10 + i;

            AnimationQueue.Instance.AddAnimToLastIndex(animRetVal1, waitFrames);
            AnimationQueue.Instance.AddAnimToLastIndex(animRetVal2, waitFrames);
        }
    }

    void MakeInitialHands()
    {
        for (int i = 0; i < Hand.INITIAL_CARDS_NUM; i++)
        {
            m_MyHand.AddCard(m_MyDeck.DrawCard());
            m_OppoHand.AddCard(m_OppoDeck.DrawCard());

            //アニメーション処理
            AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
            m_MyHand.DoAddCardAnim();
            m_OppoHand.DoAddCardAnim();
        }
    }

    void MakeInitialTrash()
    {
        m_RightTrush.AddCard(m_MyDeck.DrawCard());
        m_LeftTrush.AddCard(m_OppoDeck.DrawCard());

        //アニメーション処理
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
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
                AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                if (dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
                {
                    dealDeck.AddCard(dealTrush.DrawCard());

                    //TODO ジョーカーが一番上表示になってない?
                    AnimationQueue.Instance.AddAnimToLastIndex(
                        dealDeck.m_cards[dealDeck.m_cards.Count - 1].Anim_StraightLineMoveWithTurnOver(dealDeck.transform.position), 60);
                    dealDeck.Shuffle();

                    dealTrush.AddCard(dealDeck.DrawCard());

                    //アニメーション処理
                    AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                    dealTrush.DoDiscardAnim(true);
                }
            }
            dealDeck = m_OppoDeck;
            dealTrush = m_LeftTrush;
        }
    }
}

