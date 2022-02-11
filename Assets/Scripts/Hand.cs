using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : HoldCardObject
{
    List<Card> m_cards;

    public const int MAX_CARDS_NUM = 6;
    public const int INITIAL_CARDS_NUM = 4;
    public const float SPACE_BETWEEN_CARDS = 2.5f;

    const int frameToSpend = 20;
    public bool m_isFront { get; private set; }

    private void Awake()
    {
        m_cards = new List<Card>();
        DisableReceiveDrop();
        m_isFront = true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool CanAddCard()
    {
        return m_cards.Count < MAX_CARDS_NUM;
    }

    override public void AddCard(Card card, bool doAnim = true)
    {
        if (!CanAddCard())
        {
            Debug.LogError("Hand can't have more card than MAX_CARDS_NUM.");
        }
        card.transform.SetParent(this.transform.Find("Canvas").transform);
        m_cards.Add(card);
        card.transform.SetAsLastSibling();

        // アニメーション処理
        if (doAnim)
            DoAddCardAnim();
    }
    public override void RemoveCard(Card card, bool doAnim)
    {
        m_cards.Remove(card);
        DoMoveCardsAnim();
    }

    public void DoMoveCardsAnim()
    {
        //カードの移動
        for (int i = 0; i < m_cards.Count; i++)
        {
            Vector3 afterPosition = CalcCardPosition()[i];
            IEnumerator<bool> animRetVal = m_cards[i].Anim_StraightLineMove(afterPosition, frameToSpend);
            AnimationQueue.Instance.AddAnimToLastIndex(animRetVal);
        }
    }

    // AddCardを行った後
    public void DoAddCardAnim()
    {
        //移動
        DoMoveCardsAnim();
        //カードの裏返し
        if (m_cards[m_cards.Count - 1].m_isFront != this.m_isFront)
        {
            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[m_cards.Count - 1].Anim_TurnOver(frameToSpend));
        }
    }

    public List<Vector3> CalcCardPosition()
    {
        List<Vector3> poses = new List<Vector3>();

        float cardxPos = 0;
        switch(m_cards.Count % 2)
        {
            case 0: //偶数
                cardxPos = -1 * (-SPACE_BETWEEN_CARDS / 2 + SPACE_BETWEEN_CARDS * m_cards.Count / 2);
                break;
            case 1: //奇数
                cardxPos = -1 * (SPACE_BETWEEN_CARDS * (m_cards.Count - 1) / 2);
                break;
        }

        for(int i = 0; i < m_cards.Count; i++)
        {
            poses.Add(transform.TransformPoint(new Vector3(cardxPos, 0, 0)));
            cardxPos += SPACE_BETWEEN_CARDS;
        }
        return poses;
    }

    public override void CardDrop(Card droppedCard)
    {
        base.CardDrop(droppedCard);
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        AddCard(droppedCard);

        //DoAddCardAnim();
    }
}
