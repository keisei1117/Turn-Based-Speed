using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class Deck : HoldCardObject, IDropHandler
{
    public bool m_canDrop;
    public bool m_isFront { get; protected set; }

    //m_cards[0]����ԉ�
    public List<Card> m_cards { get; protected set; }

    private void Awake()
    {
        m_cards = new List<Card>();
        m_canDrop = false;
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
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log(gameObject.name + "��" + eventData.pointerDrag.name + "���h���b�v����܂����B");
        //foreach (GameObject item in eventData.hovered)
        //    Debug.Log(gameObject.name + "��" + item.name + "���h���b�v����܂����B");
    }

    // �J�[�h��������
    override public void AddCard(Card card, bool doAnim = true)
    {
        Transform canvas = card.transform.parent; // Canvas <- Card
        if (canvas != null)
        {
            Transform parent = canvas.parent; // HoldCardObject <- Canvas
            //Debug.Log(card.name + "'s parent is " + parent.name);
            parent.GetComponent<HoldCardObject>().RemoveCard(card, true);
        }

        card.transform.SetParent(this.transform.Find("Canvas").transform); // Card�̐e�����̃I�u�W�F�N�g�ɐݒ�
        m_cards.Add(card);
        card.transform.SetAsLastSibling();//�`�ʂ���ԍŌ�ɐݒ�

        if (doAnim)
        {
            // �A�j���[�V��������
            if (card.m_isFront == this.m_isFront)
            {
                AnimationQueue.Instance.AddAnimToLastIndex(card.Anim_StraightLineMove(this.gameObject.transform.position));
            }
            else
            {
                AnimationQueue.Instance.AddAnimToLastIndex(card.Anim_StraightLineMoveWithTurnOver(this.gameObject.transform.position));
            }
        }
    }
    public override void RemoveCard(Card card, bool doAnim)
    {
        m_cards.Remove(card);
    }

    // ��ԏ��Card��Ԃ�
    public Card GetTopCard()
    {
        int lastIndex = m_cards.Count - 1;
        return m_cards[lastIndex];
    }

    // �J�[�h��1������
    // �߂�l�ɋA����Card�͂���Deck������������
    public Card DrawCard()
    {
        int lastIndex = m_cards.Count - 1;
        Card c = m_cards[lastIndex];
        m_cards.RemoveAt(lastIndex);
        
        return c;
    }

    //���݂̃J�[�h�̏��Ԃ����O�ɏo��
    public void LogNowOrder()
    {
        string str = this.name + ": Deck's Order\n";
        for(int i = 0; i < m_cards.Count; i++)
        {
            str += "\tm_cards[" + i.ToString() + "]: ";
            str += m_cards[i].GetComponent<Card>().GetImage().name;
            str += "\n";
        }
        Debug.Log(str);
    }

    //�V���b�t������@�A�j���[�V�������o�^����
    public void Shuffle()
    {
        //Debug.Log(this.name + ": Shuffling");
        List<Card> ShuffledCards = new List<Card>();
        while (m_cards.Count != 0)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            int i = Random.Range(0, m_cards.Count);

            ShuffledCards.Add(m_cards[i]);
            m_cards.RemoveAt(i);
        }

        m_cards = ShuffledCards;

        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        AnimationQueue.Instance.AddAnimToLastIndex(Anim_Shuffle());
    }

    //�V���b�t���̃A�j���[�V�������\�b�h
    IEnumerator<bool> Anim_Shuffle()
    {
        if (m_cards.Count < 3) //TODO: 2���̎��̃A�j���[�V����
        {
            Debug.Log("can't shuffle");
            yield return true;
        }

        Card animCard1 = m_cards[m_cards.Count - 1];
        Card animCard2 = m_cards[m_cards.Count - 2];
        Card animCard3 = m_cards[m_cards.Count - 3];

        const float distance = 0.2f;
        const int loopCount = 10;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(distance, 0f, 0f);
                animCard2.transform.Translate(-distance, 0f, 0f);
                yield return false;
            }

            // �\���ɂ�����㉺�֌W�̐ݒ�
            animCard1.transform.SetAsLastSibling();
            animCard3.transform.SetAsLastSibling();
            animCard2.transform.SetAsLastSibling();

            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(-distance, 0f, 0f);
                animCard2.transform.Translate(distance, 0f, 0f);
                yield return false;
            }

            animCard3.transform.SetAsLastSibling();
            animCard2.transform.SetAsLastSibling();
            animCard1.transform.SetAsLastSibling();
        }
        //Debug.Log("Anim_Shuffle end");
        yield return true;
    }

    public void SetViewOrder()
    {
        for (int i = 0; i < m_cards.Count; i++)
        {
            m_cards[i].transform.SetAsFirstSibling();
        }
    }

    override public void CardDrop(Card droppedCard)
    {
        base.CardDrop(droppedCard);
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        this.AddCard(droppedCard);
        //AnimationQueue.Instance.AddAnimToLastIndex(droppedCard.Anim_StraightLineMoveWithTurnOver(this.gameObject.transform.position));
    }

}
