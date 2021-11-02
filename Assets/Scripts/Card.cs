using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
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
    public Image m_attachedObject;

    static Sprite[] m_sprites;

    private void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    private void Update()
    {
        if (m_attachedObject != null)
        {
            m_attachedObject.sprite = GetImage();
        }
    }

    public void Initialize(Image imageObject, Suit suitVal, int numVal, bool isFront = false)
    {
        m_attachedObject = imageObject;
        m_suit = suitVal;
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

    public IEnumerator<bool> Anim_StraightLineMove(Vector3 afterPosition, int frameToSpend = 20)
    {
        Debug.Log("Anim_StraightLineMove start");

        Vector3 distOfFrame = (afterPosition - this.transform.position) / frameToSpend;
        for(int i = 0; i < frameToSpend; i++)
        {
            this.transform.Translate(distOfFrame);
            yield return false;
        }
        this.transform.SetPositionAndRotation(afterPosition, new Quaternion());
        yield return false;

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
