using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour,
                    IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public enum Suit
    {
        Club,
        Diamond,
        Heart,
        Spade,
        Joker,
    }

    public Suit m_suit { get; private set; }
    public int m_num { get; private set; }
    public bool m_isFront { get; private set; }
    public Image m_attachedObject;

    static Sprite[] m_sprites;

    Vector3 m_beforeDragPos;
    Transform m_beforeDragParent;

    //flags
    public bool m_isDragging { get; private set; } = false;
    public bool m_canDrag;

    private void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        m_canDrag = false;
    }

    private void Update()
    {
    }

    //TODO:ドラッグの処理
    public void OnDrag(PointerEventData eventData)
    {
        if (!m_canDrag) return;
        Debug.Log("OnDrag");
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(eventData.position);
        targetPos.z = 0;
        this.transform.position = targetPos;
        m_isDragging = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_beforeDragPos = this.transform.position;
        m_beforeDragParent = this.transform.parent;
        this.transform.SetParent(GameManager.Instance.m_TopLayerCanvas.transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        this.transform.SetParent(m_beforeDragParent);
        //this.transform.SetAsLastSibling(); 使わなくても勝手に一番下に設定される
        AnimationManager.Instance.AddAnimToFirstIndex(
            Anim_StraightLineMove(m_beforeDragPos, 7));
        m_isDragging = false;
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

    public IEnumerator<bool> Anim_StraightLineMoveWithTurnOver(Vector3 afterPosition, int framToSpend = 20)
    {
        IEnumerator<bool> lineMove = Anim_StraightLineMove(afterPosition, framToSpend);
        IEnumerator<bool> turnOver = Anim_TurnOver(framToSpend);

        while(!lineMove.Current || !turnOver.Current)
        {
            lineMove.MoveNext();
            turnOver.MoveNext();
            yield return false;
        }
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
