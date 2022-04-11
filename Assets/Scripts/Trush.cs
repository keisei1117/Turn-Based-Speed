using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trush : Deck
{
    public GameObject mask;

    private void Awake()
    {
        m_cards = new List<Card>();
        m_canDrop = false;
        DisableReceiveDrop();
        m_isFront = true;
        DisableMask();
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //public void DoDiscardAnim(bool ifTurnover)
    //{
    //    if (ifTurnover)
    //    {
    //        AnimationQueue.Instance.AddAnimToLastIndex(
    //            m_cards[m_cards.Count - 1].Anim_StraightLineMoveWithTurnOver(this.transform.position));
    //    }
    //    else
    //    {
    //        AnimationQueue.Instance.AddAnimToLastIndex(
    //            m_cards[m_cards.Count - 1].Anim_StraightLineMove(this.transform.position));
    //    }
    //}

    public override void AddCard(Card card, bool doAnim = true)
    {
        mask.transform.SetAsLastSibling();
        base.AddCard(card, doAnim);
    }

    public void EnableMask()
    {
        mask.SetActive(true);
    }
    public void DisableMask()
    {
        mask.SetActive(false);
    }
}
