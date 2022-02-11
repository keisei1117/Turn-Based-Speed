using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trush : Deck
{
    private void Awake()
    {
        m_cards = new List<Card>();
        m_canDrop = false;
        DisableReceiveDrop();
        m_isFront = true;
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DoDiscardAnim(bool ifTurnover)
    {
        if (ifTurnover)
        {
            AnimationQueue.Instance.AddAnimToLastIndex(
                m_cards[m_cards.Count - 1].Anim_StraightLineMoveWithTurnOver(this.transform.position));
        }
        else
        {
            AnimationQueue.Instance.AddAnimToLastIndex(
                m_cards[m_cards.Count - 1].Anim_StraightLineMove(this.transform.position));
        }
    }
}
