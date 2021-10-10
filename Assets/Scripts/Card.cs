using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public enum Suit
    {
        Club,
        Diamond,
        Heart,
        Spade,
        Joker,
    }

    Suit m_suit;
    int m_num;
    bool m_isFront;
    static Sprite[] m_sprites;

    public Card(Suit SuitVal, int numVal, bool isFront)
    {
        m_suit = SuitVal;
        m_num = numVal;
        m_isFront = isFront;
        LoadImages();
    }

    static public void LoadImages()
    {
        if (m_sprites == null)
        {
            Debug.Log("Loading CardImage");
            m_sprites = Resources.LoadAll<Sprite>("Images/Playing Cards");
            //for (int i = 0; i < m_sprites.Length; i++)
            //{
            //    Debug.Log("m_sprites[" + i.ToString() + "]: " + m_sprites[i].name);
            //}
        }
    }

    public Sprite GetImage()
    {
        string fileName = "";
        switch (m_suit)
        {
            case Suit.Club:
                fileName = "Club";
                break;
            case Suit.Diamond:
                fileName = "Diamond";
                break;
            case Suit.Heart:
                fileName = "Heart";
                break;
            case Suit.Spade:
                fileName = "Spade";
                break;
        }
        fileName += m_num.ToString("00");
        if (m_suit == Suit.Joker)
        {
            switch (m_num)
            {
                case 1:
                    fileName = "Joker_Color";
                    break;
                case 2:
                    fileName = "Joker_Monochrome";
                    break;
            }
        }

        if (!m_isFront)
            fileName = "BackColor_Black";

        foreach (Sprite sprite in m_sprites)
        {
            if(sprite.name == fileName)
            {
                return sprite;
            }
        }

        throw new System.Exception("The image file whom name is \"" + fileName + "\" can't be loaded.");
    }
}
