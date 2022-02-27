using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : HoldCardObject
{
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

        base.AddCard(card, doAnim);

        // �A�j���[�V��������
        if (doAnim)
            DoAddCardAnim();
    }
    public override void RemoveCard(Card card, bool doAnim)
    {
        base.RemoveCard(card, doAnim);
        DoMoveCardsAnim();
    }

    public void DoMoveCardsAnim()
    {
        //�J�[�h�̈ړ�
        for (int i = 0; i < m_cards.Count; i++)
        {
            Vector3 afterPosition = CalcCardPosition()[i];
            IEnumerator<bool> animRetVal = m_cards[i].Anim_StraightLineMove(afterPosition, frameToSpend);
            AnimationQueue.Instance.AddAnimToLastIndex(animRetVal);
        }
    }

    // AddCard���s������
    public void DoAddCardAnim()
    {
        //�ړ�
        DoMoveCardsAnim();
        //�J�[�h�̗��Ԃ�
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
            case 0: //����
                cardxPos = -1 * (-SPACE_BETWEEN_CARDS / 2 + SPACE_BETWEEN_CARDS * m_cards.Count / 2);
                break;
            case 1: //�
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
    public int GetSingleCardNum()
    {
        int count = 0;
        foreach (Card card in m_cards)
        {
            if (card.m_mode == Card.MODE.SINGLE) count++;
        }
        return count;
    }
    public int GetCanCombineOrCompressCardNum() // �W���[�J�[������
    {
        int count = 0;
        foreach(Card card in m_cards)
        {
            if (card.m_mode == Card.MODE.SINGLE && card.m_suit != Card.Suit.Joker) count++;
        }
        return count;
    }

    public void SetAllSingleCardsMode(Card.MODE mode)
    {
        foreach(Card card in m_cards)
        {
            if (card.m_mode == Card.MODE.SINGLE)
                card.SetMode(mode);
        }
    }
    public void SetAllWaitCardModeToSingle()
    {
        foreach(Card card in m_cards)
        {
            if (card.m_mode == Card.MODE.WAIT_COMBINE || card.m_mode == Card.MODE.WAIT_COMPRESS)
                card.SetMode(Card.MODE.SINGLE);
        }
    }
}
