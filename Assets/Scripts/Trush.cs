using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trush : Deck
{
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

    public Card GetTopCard()
    {
        return m_cards[m_cards.Count - 1];
    }
}
