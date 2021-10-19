using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    static public IEnumerator<bool> Anim_StraightLineMove(Sprite cardimage, Vector3 beforeCoordinate, Vector3 afterCoordinate, Transform parentObjectCanvas)
    {
        Debug.Log("Anim_StraightLineMove start");

        Image animImage = Object.Instantiate(GameManager.Instance.m_ImageCardPrefab, beforeCoordinate, new Quaternion(0,0,0,0), parentObjectCanvas);
        animImage.name = "animImage";
        animImage.enabled = true;
        animImage.sprite = cardimage;
        yield return false;


        const int requiredFrame = 20;
        Vector3 distOfFrame = (afterCoordinate - beforeCoordinate) / requiredFrame;
        for(int i = 0; i < requiredFrame; i++)
        {
            animImage.transform.Translate(distOfFrame);
            yield return false;
        }
        animImage.transform.SetPositionAndRotation(afterCoordinate, new Quaternion());
        yield return false;


        Object.Destroy(animImage);
        Debug.Log("Anim_StraightLineMove end");
        yield return true;
    }

    public void TurnOver()
    {
        m_isFront = !m_isFront;
    }

    public void TurnIntoFront()
    {
        m_isFront = true;
    }

    public void TurnIntoBack()
    {
        m_isFront = false;
    }
}
