﻿using UnityEngine;
using UnityEngine.UI;

public abstract class HoldCardObject : MonoBehaviour
{
    bool m_canReceiveDrop;
    public GameObject m_dropRange;
    public GameObject m_canDropSign;

    abstract public void AddCard(Card card, bool doAnim);
    abstract public void RemoveCard(Card card, bool doAnim);

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