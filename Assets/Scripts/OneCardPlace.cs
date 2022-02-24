using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCardPlace: HoldCardObject
{
    Card m_Cards;
    int m_num;

    public GameObject m_canDragSign;
    public bool m_canDrag { get; private set; }

    public enum MODE
    {
        SINGLE,
        COMBINED,
        COMPRESSED,
        WAIT_COMBINE,
        WAIT_COMPRESS
    }
    MODE m_mode;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMode(MODE mode)
    {
        m_mode = mode;
    }

    public override void AddCard(Card card, bool doAnim)
    {
        Debug.Assert(m_mode == MODE.SINGLE || m_mode == MODE.WAIT_COMBINE || m_mode == MODE.WAIT_COMBINE);

        base.AddCard(card, doAnim);

        //Single
        if(m_mode == MODE.SINGLE)
        {
            Debug.Assert(m_cards.Count == 1);

            m_num = m_cards[0].m_num;

            if(doAnim)
            {
                if (m_cards[0].m_isFront)
                {
                    AnimationQueue.Instance.AddAnimToLastIndex(m_cards[0].Anim_StraightLineMove(this.transform.position));
                }
                else
                {
                    AnimationQueue.Instance.AddAnimToLastIndex(m_cards[0].Anim_StraightLineMoveWithTurnOver(this.transform.position));
                }
            }
        }

        //Combine
        if(m_mode == MODE.WAIT_COMBINE)
        {
            Debug.Assert(m_cards.Count == 2);

            m_num = (m_cards[0].m_num + m_cards[1].m_num) % 13;
            m_mode = MODE.COMBINED;

            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[1].Anim_StraightLineMoveWithRotate(this.transform.position, this.transform.rotation.z + 90));
        }

        //Compress
        if (m_mode == MODE.WAIT_COMPRESS)
        {
            m_num = m_cards[0].m_num;
            m_mode = MODE.COMPRESSED;

            const float gap = 10;
            Vector3 pos0 = this.transform.position;
            Vector3 pos1 = this.transform.position;
            pos0.y -= gap;
            pos1.y += gap;
            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[0].Anim_StraightLineMove(pos0));
            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[1].Anim_StraightLineMove(pos1));
        }
    }

    bool IsEmpty()
    {
        return m_cards.Count == 0;
    }
}
