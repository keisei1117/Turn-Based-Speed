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
    }

    public void Initialize(Image imageObject, Suit suitVal, int numVal, bool isFront = false)
    {
        m_attachedObject = imageObject;
        m_suit = suitVal;
        m_num = numVal;
        m_isFront = isFront;
        LoadImages();
        m_attachedObject.sprite = GetImage();
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
        for (int i = 0; i < frameToSpend; i++)
        {
            this.transform.position += distOfFrame;
            yield return false;
        }
        this.transform.SetPositionAndRotation(afterPosition, new Quaternion());
        
        Debug.Log("Anim_StraightLineMove end");
        yield return true;
    }

    public IEnumerator<bool> Anim_TurnOver(int frameToSpend = 20)
    {
        Debug.Log("Anim_TurnOver start");
        int BeforeTurnOverframe = frameToSpend / 2;
        int AfterTurnOverframe = frameToSpend - BeforeTurnOverframe;

        float rotateAnglePerFrame = 90 / (float)BeforeTurnOverframe;
        for (int i = 0; i < BeforeTurnOverframe; i++)
        {
            this.transform.Rotate(new Vector3(0, rotateAnglePerFrame, 0));
            yield return false;
        }

        TurnOver();

        rotateAnglePerFrame = 90 / (float)AfterTurnOverframe;
        for (int i = 0; i < AfterTurnOverframe; i++)
        {
            this.transform.Rotate(new Vector3(0, -rotateAnglePerFrame, 0));
            yield return false;
        }
        Quaternion accurateRotarion = this.transform.rotation;
        accurateRotarion.y = 0;
        this.transform.rotation = accurateRotarion;

        Debug.Log("Anim_TurnOver end");
        yield return true;
    }

    public void TurnOver()
    {
        m_isFront = !m_isFront;
        m_attachedObject.sprite = GetImage();
    }

    public void TurnIntoFront()
    {
        m_isFront = true;
        m_attachedObject.sprite = GetImage();
    }

    public void TurnIntoBack()
    {
        m_isFront = false;
        m_attachedObject.sprite = GetImage();
    }
}
