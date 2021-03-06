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
                () =>
                {
                    AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                    m_RootDeck.Shuffle();
                },
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
        m_RightTrush.AddCard(joker1, false);
        m_LeftTrush.AddCard(joker2, false);
        joker1.TurnIntoFront();
        joker2.TurnIntoFront();

        foreach (Card card in m_RootDeck.m_cards)
        {
            card.transform.position = m_RootDeck.transform.position;
        }
        joker1.transform.position = m_RightTrush.transform.position;
        joker2.transform.position = m_LeftTrush.transform.position;

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

        //ジョーカー
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        m_MyDeck.AddCard(m_RightTrush.DrawCard());
        m_OppoDeck.AddCard(m_LeftTrush.DrawCard());

        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        m_MyDeck.Shuffle();
        m_OppoDeck.Shuffle();
    }

    void MakeInitialHands()
    {
        Debug.Log("Making Initial Hands");
        for (int i = 0; i < Hand.INITIAL_CARDS_NUM; i++)
        {
            AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
            m_MyHand.AddCard(m_MyDeck.DrawCard());
            m_OppoHand.AddCard(m_OppoDeck.DrawCard());
        }
    }

    void MakeInitialTrash()
    {
        Debug.Log("Making Initial Trash");
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        m_RightTrush.AddCard(m_MyDeck.DrawCard());
        m_LeftTrush.AddCard(m_OppoDeck.DrawCard());

        //ジョーカーが出てしまった場合
        Deck dealDeck = m_MyDeck;
        Trush dealTrush = m_RightTrush;
        for (int i = 0; i < 2; ++i)
        {
            while (dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
            {
                Debug.Log("joker is invalid for initial Trush.");

                if (dealTrush.GetTopCard().m_suit == Card.Suit.Joker)
                {
                    Card joker = dealTrush.DrawCard();
                    dealDeck.AddCard(joker, false);
                    AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                    AnimationQueue.Instance.AddAnimToLastIndex(joker.Anim_StraightLineMoveWithTurnOver(dealDeck.gameObject.transform.position), 20);

                    AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                    dealDeck.Shuffle();

                    //引き直し
                    AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                    Card newDrawCard = dealDeck.DrawCard();
                    dealTrush.AddCard(newDrawCard, false);
                    AnimationQueue.Instance.AddAnimToLastIndex(newDrawCard.Anim_StraightLineMoveWithTurnOver(dealTrush.gameObject.transform.position));
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

    bool m_drawedToTrushLastTurn;
    bool m_drawedToTrushLastMyTurn;
    bool m_drawedToTrushLastOppoTurn;

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
        m_LeftTrush.EnableMask();
        m_RightTrush.EnableMask();
        m_drawedToTrushLastTurn = false;
        m_drawedToTrushLastMyTurn = false;
        m_drawedToTrushLastOppoTurn = false;
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

    // --------------------------------------------------------------------------------------
    Deck m_handlingDeck;
    Hand m_handlingHand;
    Trush m_mainTrush, m_subTrush;
    void StartTurn()
    {
        Debug.Log("Start Turn");
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        if (m_gameStatus.IsMyTurn())
        {
            m_handlingDeck = m_MyDeck;
            m_handlingHand = m_MyHand;
            m_mainTrush = m_RightTrush;
            m_subTrush = m_LeftTrush;
            m_drawedToTrushLastTurn = m_drawedToTrushLastMyTurn;
            m_drawedToTrushLastMyTurn = false;
            AnimationQueue.Instance.AddAnimToLastIndex(m_UIManager.Anim_Transition("My Turn"));
        } else
        {
            m_handlingDeck = m_OppoDeck;
            m_handlingHand = m_OppoHand;
            m_mainTrush = m_LeftTrush;
            m_subTrush = m_RightTrush;
            m_drawedToTrushLastTurn = m_drawedToTrushLastOppoTurn;
            m_drawedToTrushLastOppoTurn = false;
            AnimationQueue.Instance.AddAnimToLastIndex(m_UIManager.Anim_Transition("Opponent Turn"));
        }


        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        WorkQueue.Instance.Stop();

        Action commonPressedBehave = () =>
        {
            WorkQueue.Instance.Restart();

            m_UIManager.DrawButton.ClearPressedBehave();
            m_UIManager.DiscardButton.ClearPressedBehave();
            m_UIManager.CombineButton.ClearPressedBehave();
            m_UIManager.CompressButton.ClearPressedBehave();
            m_UIManager.TurnEndButton.ClearPressedBehave();
            m_UIManager.DrawButton.Disable();
            m_UIManager.DiscardButton.Disable();
            m_UIManager.CombineButton.Disable();
            m_UIManager.CompressButton.Disable();
            m_UIManager.TurnEndButton.Disable();
        };

        m_UIManager.DrawButton.Disable();
        m_UIManager.DiscardButton.Disable();
        m_UIManager.CombineButton.Disable();
        m_UIManager.CompressButton.Disable();
        

        // DrawButton
        if (m_handlingDeck.m_cards.Count != 0 && !(m_drawedToTrushLastTurn && !m_handlingHand.CanAddCard()))
        {
            m_UIManager.DrawButton.Enable();
            m_UIManager.DrawButton.RegistPressedBehave(() =>
            {
                commonPressedBehave();
                WorkQueue.Instance.EnqueueOnceRunFunc(DrawFromDeck);
            });
        }

        // DiscardButton
        bool canDiscard = false;
        foreach(var card in m_handlingHand.m_cards)
        {
            if (card.IsContinuous(m_LeftTrush.GetTopCard())) canDiscard = true;
            if (card.IsContinuous(m_RightTrush.GetTopCard())) canDiscard = true;
        }
        if (m_handlingDeck.m_cards.Count == 0) canDiscard = true;
        if(canDiscard)
        {
            m_UIManager.DiscardButton.Enable();
            m_UIManager.DiscardButton.RegistPressedBehave(() =>
            {
                commonPressedBehave();
                isFirstDiscard = true;
                WorkQueue.Instance.EnqueueOnceRunFunc(DiscardPhase);
            });
        }

        // CombineButton
        if(m_handlingHand.GetCanCombineOrCompressCardNum() >= 2)
        {
            m_UIManager.CombineButton.Enable();
            m_UIManager.CombineButton.RegistPressedBehave(() =>
            {
                commonPressedBehave();
                WorkQueue.Instance.EnqueueOnceRunFunc(CombinePhase);
            });
        }

        // CompressButton
        if (CanCompress())
        {
            m_UIManager.CompressButton.Enable();
            m_UIManager.CompressButton.RegistPressedBehave(() =>
            {
                commonPressedBehave();
                WorkQueue.Instance.EnqueueOnceRunFunc(CompressPhase);
            });
        }

        // TurnEndButton
        m_UIManager.TurnEndButton.Enable();
        m_UIManager.TurnEndButton.RegistPressedBehave(() =>
        {
            commonPressedBehave();
            WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
        });
    }

    void DrawFromDeck()
    {
        Debug.Log("DrawFromDeck");

        Debug.Assert(m_handlingDeck.m_cards.Count != 0);
        Card topCard = m_handlingDeck.GetTopCard();
        Transform deckTransform = topCard.transform.parent.parent;

        topCard.EnableDrag();

        if (!m_drawedToTrushLastTurn)
        {
            topCard.RegistBeginDragObserver(m_mainTrush.EnableReceiveDrop);
            topCard.RegistEndDragObserver(m_mainTrush.DisableReceiveDrop);
            topCard.RegistBeginDragObserver(m_subTrush.EnableReceiveDrop);
            topCard.RegistEndDragObserver(m_subTrush.DisableReceiveDrop);
        }
        if (m_handlingHand.CanAddCard())
        {
            topCard.RegistBeginDragObserver(m_handlingHand.EnableReceiveDrop);
            topCard.RegistEndDragObserver(m_handlingHand.DisableReceiveDrop);
        }
        Card.EnqueueHappenHandlingObserver(topCard.DisableDrag);
        Card.EnqueueHappenHandlingObserver(topCard.ClearDragObserverList);


        WorkQueue.Instance.Stop();
        Card.EnqueueHappenHandlingObserver(WorkQueue.Instance.Restart);


        Card.EnqueueHappenHandlingObserver(() =>
        {
            if (topCard.GetParentHoldCardObject() == m_handlingHand) //手札にドローしたとき
                {
                    // INITIAL_CARDS_NUMの枚数まで自動で引く
                    int drawNum = Hand.INITIAL_CARDS_NUM - m_handlingHand.m_cards.Count;
                if (drawNum > m_handlingDeck.m_cards.Count) drawNum = m_handlingDeck.m_cards.Count;
                if (drawNum > 0)
                {
                    for (int i = 0; i < drawNum; i++)
                    {
                        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                        m_handlingHand.AddCard(m_handlingDeck.DrawCard());
                    }
                }
            }

            bool drawedToTrush =
                topCard.GetParentHoldCardObject() == m_mainTrush || topCard.GetParentHoldCardObject() == m_subTrush;
            if (m_gameStatus.IsMyTurn())
            {
                m_drawedToTrushLastMyTurn = drawedToTrush;
            }
            else
            {
                m_drawedToTrushLastOppoTurn = drawedToTrush;
            }
        });
    
        WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
    }

    bool isFirstDiscard;
    void DiscardPhase()
    {
        Debug.Log("Discard Phase");

        // バースト
        if (m_handlingHand.m_cards.Count == 0 && m_handlingDeck.m_cards.Count != 0)
        {
            Debug.Log("Burst happen.");

            // INITIAL_CARDS_NUMの枚数まで自動で引く
            int drawNum = Hand.INITIAL_CARDS_NUM;
            if (drawNum > m_handlingDeck.m_cards.Count) drawNum = m_handlingDeck.m_cards.Count;
            for (int i = 0; i < drawNum; i++)
            {
                AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
                m_handlingHand.AddCard(m_handlingDeck.DrawCard());
            }
            WorkQueue.Instance.EnqueueOnceRunFunc(DiscardPhase);
            return;
        }
        // 決着
        if (m_handlingDeck.m_cards.Count == 0 && m_handlingHand.m_cards.Count == 0)
        {
            bool isPlayerWinner = m_gameStatus.IsMyTurn();
            WorkQueue.Instance.EnqueueOnceRunFunc(() =>
            {
                nextState = new ResultState(
                    m_RootDeck, m_MyDeck, m_OppoDeck, m_RightTrush, m_LeftTrush, m_MyHand, m_OppoHand, m_UIManager, m_gameStatus,
                    isPlayerWinner);
            });
            return;
        }

        
        if(isFirstDiscard && m_handlingDeck.m_cards.Count == 0) //山札がもうないとき
        {
            Debug.Log("No cards in Deck");
            foreach (var card in m_handlingHand.m_cards)
            {
                card.EnableDrag();

                card.RegistBeginDragObserver(m_mainTrush.EnableReceiveDrop);
                card.RegistEndDragObserver(m_mainTrush.DisableReceiveDrop);
                card.RegistBeginDragObserver(m_subTrush.EnableReceiveDrop);
                card.RegistEndDragObserver(m_subTrush.DisableReceiveDrop);
                Card.EnqueueHappenHandlingObserver(card.DisableDrag);
                Card.EnqueueHappenHandlingObserver(card.ClearDragObserverList);
            }
        }
        else //通常時
        {
            SetContinuousRelation(isFirstDiscard);
        }

        isFirstDiscard = false;

        bool canDiscard = false;
        foreach (var card in m_handlingHand.m_cards)
        {
            if (card.m_canDrag) canDiscard = true;
        }

        if (canDiscard)
        {
            Card.EnqueueHappenHandlingObserver(() =>
            {
                WorkQueue.Instance.EnqueueOnceRunFunc(DiscardPhase);
            });

            // TurnEndButtonを押された時の挙動
            m_UIManager.TurnEndButton.Enable();
            m_UIManager.TurnEndButton.RegistPressedBehave(() =>
            {
                WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
                foreach (var card in m_handlingHand.m_cards)
                {
                    card.DisableDrag();
                    card.ClearDragObserverList();
                    Card.ClearHappenHandlingObserver();
                }
            });
        }
        else
        {
            // 強制ターンエンド
            WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
        }
    }

    void CombinePhase()
    {
        Debug.Log("Combine Phase");
        m_handlingHand.SetAllSingleCardsMode(Card.MODE.WAIT_COMBINE);
        Card.ClearHappenHandlingObserver();

        foreach (var card in m_handlingHand.m_cards)
        {
            if (card.m_mode != Card.MODE.WAIT_COMBINE || card.m_suit == Card.Suit.Joker)
                continue;
            card.EnableDrag();
            Card.EnqueueHappenHandlingObserver(card.DisableDrag);
            Card.EnqueueHappenHandlingObserver(card.ClearDragObserverList);
            foreach (var targetCard in m_handlingHand.m_cards)
            {
                if (targetCard.m_mode != Card.MODE.WAIT_COMBINE || targetCard.m_suit == Card.Suit.Joker) 
                    continue;
                if (card != targetCard)
                {
                    card.RegistBeginDragObserver(targetCard.EnableReceiveDrop);
                    card.RegistEndDragObserver(targetCard.DisableReceiveDrop);
                }
            }
        }

        // TurnEndButtonを押された時の挙動
        m_UIManager.TurnEndButton.Enable();
        m_UIManager.TurnEndButton.RegistPressedBehave(() =>
        {
            m_handlingHand.SetAllWaitCardModeToSingle();
            foreach (var card in m_handlingHand.m_cards)
            {
                card.DisableDrag();
                card.ClearDragObserverList();
                Card.ClearHappenHandlingObserver();
            }
            WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
        });

        Card.EnqueueHappenHandlingObserver(() =>
        {
            m_handlingHand.SetAllWaitCardModeToSingle();
            //Debug.Log("GetSingleCardNum(): " + m_handlingHand.GetCanCombineOrCompressCardNum());

            if (m_handlingHand.GetCanCombineOrCompressCardNum() >= 2)
            {
                WorkQueue.Instance.EnqueueOnceRunFunc(CombinePhase);
            }
            else
            {
                WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
            }
        });
    }

    void CompressPhase()
    {
        Debug.Log("Compress Phase");
        Card.ClearHappenHandlingObserver();

        if(!CanCompress())
        {
            foreach(Card card in m_handlingHand.m_cards)
            {
                if (card.m_mode == Card.MODE.COMPRESSING)
                    card.SetMode(Card.MODE.COMPRESSED);
            }
            WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
            return;
        }

        foreach (Card card1 in m_handlingHand.m_cards)
        {
            foreach(Card card2 in m_handlingHand.m_cards)
            {
                if(card2.CanCompressTothis(card1))
                {
                    card1.SetMode(Card.MODE.WAIT_COMPRESS);
                }
            }
        }

        foreach (var card in m_handlingHand.m_cards)
        {
            if (card.m_mode != Card.MODE.WAIT_COMPRESS)
                continue;
            card.EnableDrag();
            Card.EnqueueHappenHandlingObserver(card.DisableDrag);
            Card.EnqueueHappenHandlingObserver(card.ClearDragObserverList);
            foreach (var targetCard in m_handlingHand.m_cards)
            {
                if(targetCard.CanCompressTothis(card))
                {
                    card.RegistBeginDragObserver(targetCard.EnableReceiveDrop);
                    card.RegistEndDragObserver(targetCard.DisableReceiveDrop);
                }
            }
        }

        // TurnEndButtonを押された時の挙動
        m_UIManager.TurnEndButton.Enable();
        m_UIManager.TurnEndButton.RegistPressedBehave(() =>
        {
            m_handlingHand.SetAllWaitCardModeToSingle();
            foreach (Card card in m_handlingHand.m_cards)
            {
                if (card.m_mode == Card.MODE.COMPRESSING)
                    card.SetMode(Card.MODE.COMPRESSED);
            }
            foreach (var card in m_handlingHand.m_cards)
            {
                card.DisableDrag();
                card.ClearDragObserverList();
                Card.ClearHappenHandlingObserver();
            }
            WorkQueue.Instance.EnqueueOnceRunFunc(TurnEnd);
        });

        Card.EnqueueHappenHandlingObserver(() =>
        {
            m_handlingHand.SetAllWaitCardModeToSingle();
            Debug.Log("GetSingleCardNum(): " + m_handlingHand.GetCanCombineOrCompressCardNum());

            WorkQueue.Instance.EnqueueOnceRunFunc(CompressPhase);
        });
    }

    Card LastLeftTrush, lastRightTrush;
    void SetContinuousRelation(bool isFirst)
    {
        //2回目以降は1回目の時出した方にのみ出せる
        bool isLeftEnabled = true;
        bool isRightEnabled = true;
        if(!isFirst)
        {
            isLeftEnabled = LastLeftTrush != m_LeftTrush.GetTopCard();
            isRightEnabled = lastRightTrush != m_RightTrush.GetTopCard();
        }

        foreach (var card in m_handlingHand.m_cards)
        {
            //Debug.Log("setting " + card.name + " 's continuous relation.");

            bool isContinuousWithLeft = card.IsContinuous(m_LeftTrush.GetTopCard());
            bool isContinuousWithRight = card.IsContinuous(m_RightTrush.GetTopCard());

            isContinuousWithLeft &= isLeftEnabled;
            isContinuousWithRight &= isRightEnabled;

            if (isContinuousWithLeft || isContinuousWithRight)
            {
                //Debug.Log(card.name + " is continuous.");

                card.EnableDrag();
                Card.EnqueueHappenHandlingObserver(card.DisableDrag);
                Card.EnqueueHappenHandlingObserver(card.ClearDragObserverList);
            }
            if (isContinuousWithLeft)
            {
                card.RegistBeginDragObserver(m_LeftTrush.EnableReceiveDrop);
                card.RegistEndDragObserver(m_LeftTrush.DisableReceiveDrop);
            }
            if (isContinuousWithRight)
            {
                card.RegistBeginDragObserver(m_RightTrush.EnableReceiveDrop);
                card.RegistEndDragObserver(m_RightTrush.DisableReceiveDrop);
            }
        }
        LastLeftTrush = m_LeftTrush.GetTopCard();
        lastRightTrush = m_RightTrush.GetTopCard();
    }

    bool CanCompress()
    {
        bool canCompress = false;
        foreach (Card card in m_handlingHand.m_cards)
        {
            if (card.m_mode == Card.MODE.COMBINED || card.m_mode == Card.MODE.COMPRESSED) continue;
            int num = card.m_num;
            foreach (Card compareCard in m_handlingHand.m_cards)
            {
                if (card.m_suit == Card.Suit.Joker || compareCard.m_suit == Card.Suit.Joker) continue;
                if (card == compareCard) continue;
                if (compareCard.m_mode == Card.MODE.COMBINED || compareCard.m_mode == Card.MODE.COMPRESSED || 
                    compareCard.m_mode == Card.MODE.COMPRESSING) 
                    continue;
                if (num == compareCard.m_num)
                {
                    canCompress = true;
                    break;
                }
            }
        }
        return canCompress;
    }

    void TurnEnd()
    {
        Debug.Log("Turn End");

        m_UIManager.TurnEndButton.Disable();
        m_UIManager.TurnEndButton.ClearPressedBehave();
        m_gameStatus.SwitchTurn();
        WorkQueue.Instance.EnqueueOnceRunFunc(StartTurn);
    }
}

class ResultState : GameManagerState
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

    bool m_isPlayerWinner;

    public ResultState(
        Deck rootDeck, Deck myDeck, Deck oppoDeck, Trush rightTrush, Trush leftTrush, Hand myHand, Hand oppoHand, UIManager uiManager, GameStatus gameStatus,
        bool isPlayerWinner)
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

        m_isPlayerWinner = isPlayerWinner;
    }
    //------------------------------------------------------------------------------------
    public override void Enter()
    {
        m_UIManager.ShowResult(m_isPlayerWinner);
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
}
