using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class GameManager : MonoBehaviour
{
    public GameObject m_RootDeck;
    public GameObject m_MyDeck;
    public GameObject m_OppoDeck;
    public GameObject m_RightTrush;
    public GameObject m_LeftTrush;
    public GameObject m_MyHand;
    public GameObject m_OppoHand;

    public GameObject m_AnimatinoManager;

    enum GameMode
    {
        STANDBY,
        SETTING,
        PLAYING,
        POUSE,
        WAITING_ANIMATION,
    }
    GameMode m_nowMode, m_preMode;

    private void Awake()
    {
        if (m_RootDeck == null ||
            m_MyDeck == null ||
            m_OppoDeck == null ||
            m_RightTrush == null ||
            m_LeftTrush == null ||
            m_MyHand == null ||
            m_OppoDeck == null)
        {
            throw new NullReferenceException("Game Object instance is null.");
        }

        m_nowMode = GameMode.SETTING;
        m_preMode = GameMode.STANDBY;
    }

    // Start is called before the first frame update
    void Start()
    {
        Card.LoadImages();
        //MakeAllCardsToRootDeck();
        ////m_RootDeck.GetComponent<Deck>().LogNowOrder();
        //m_RootDeck.GetComponent<Deck>().Shuffle();
        ////m_RootDeck.GetComponent<Deck>().LogNowOrder();
        //MakeMyOppoDeck();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_nowMode)
        {
            case GameMode.SETTING:
                Debug.Log("Gamemode: SETTING");

                MakeAllCardsToRootDeck();
                //m_RootDeck.GetComponent<Deck>().LogNowOrder();
                m_RootDeck.GetComponent<Deck>().Shuffle();
                //m_RootDeck.GetComponent<Deck>().LogNowOrder();
                MakeMyOppoDeck();

                m_nowMode = GameMode.WAITING_ANIMATION;
                break;

            case GameMode.WAITING_ANIMATION:
                Debug.Log("Gamemode: WAITING_ANIMATION");
                if (!m_AnimatinoManager.GetComponent<AnimationManager>().IfAnimationPlaying())
                {
                    m_nowMode = m_preMode;
                }
                break;
        }
    }

    void MakeAllCardsToRootDeck()
    {
        Debug.Log("Making All Root Deck's Cards");

        for (Card.Suit s = Card.Suit.Club; s <= Card.Suit.Spade; s++)
        {
            for (int i = 1; i <= 13; i++)
            {
                m_RootDeck.GetComponent<Deck>().AddCard(new Card(s, i, false));
            }
        }
        m_RootDeck.GetComponent<Deck>().AddCard(new Card(Card.Suit.Joker, 1, false));
        m_RootDeck.GetComponent<Deck>().AddCard(new Card(Card.Suit.Joker, 2, false));
    }

    public void MakeMyOppoDeck()
    {
        Debug.Log("Making MyDeck and OppoDeck");

        int nLoop = m_RootDeck.GetComponent<Deck>().m_cards.Count / 2;
        for (int i = 0; i < nLoop; i++)
        {
            m_MyDeck.GetComponent<Deck>().AddCard(m_RootDeck.GetComponent<Deck>().DrawCard());
            m_OppoDeck.GetComponent<Deck>().AddCard(m_RootDeck.GetComponent<Deck>().DrawCard());
        }
    }
}
