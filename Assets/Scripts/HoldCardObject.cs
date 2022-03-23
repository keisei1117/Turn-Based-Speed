using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HoldCardObject : MonoBehaviour
{
    //m_cards[0]が一番下
    public List<Card> m_cards;// { get; protected set; }

    bool m_canReceiveDrop;
    public GameObject m_dropRange;
    public GameObject m_canDropSign;

    virtual public void AddCard(Card card, bool doAnim)
    {
        Transform canvas = card.transform.parent; // Canvas <- Card
        if (canvas != null)
        {
            Transform parent = canvas.parent; // HoldCardObject <- Canvas
            //Debug.Log(card.name + "'s parent is " + parent.name);
            parent.GetComponent<HoldCardObject>().RemoveCard(card, true);
        }

        card.transform.SetParent(this.transform.Find("Canvas").transform); // Cardの親をこのオブジェクトに設定
        m_cards.Add(card);
        card.transform.SetAsLastSibling(); // 描写を一番最後に設定

        if(card.m_mode == Card.MODE.COMBINED || card.m_mode == Card.MODE.COMPRESSED)
        {
            foreach(Card containedCard in card.m_cards)
            {
                containedCard.transform.SetParent(this.transform.Find("Canvas").transform); // Cardの親をこのオブジェクトに設定
                containedCard.transform.SetAsLastSibling(); // 描写を一番最後に設定
            }
        }
        m_dropRange.transform.SetAsLastSibling();
        m_canDropSign.transform.SetAsLastSibling();
    }

    virtual public void RemoveCard(Card card, bool doAnim)
    {
        //Debug.Log("Remove " + card.ToString() + " from " + this.name);
        m_cards.Remove(card);
    }

    public void EnableReceiveDrop()
    {
        m_canReceiveDrop = true;
        m_canDropSign.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;
    }
    public void DisableReceiveDrop()
    {
        m_canReceiveDrop = false;
        m_canDropSign.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = false;
    }
    virtual public void CardHover()
    {
        m_dropRange.GetComponent<Image>().enabled = true;
    }
    virtual public void CardNotHover()
    {
        m_dropRange.GetComponent<Image>().enabled = false;
    }
    virtual public void CardDrop(Card droppedCard)
    {
        m_dropRange.GetComponent<Image>().enabled = false;
    }
}
