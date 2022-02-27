using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;

public class Card : HoldCardObject,
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
    public GameObject m_canDragSign;

    static Sprite[] m_sprites;

    public enum MODE
    {
        SINGLE,
        COMBINED,
        COMPRESSED,
        WAIT_COMBINE,
        WAIT_COMPRESS,
        CONTAINED
    }
    //public MODE m_mode { get; private set; }
    public MODE m_mode;

    //flags
    public bool m_isDragging { get; private set; } = false;
    public bool m_canDrag { get; private set; }

    private void Awake()
    {
        DisableReceiveDrop();
        m_beginDragObserverList = new List<Action>();
        m_endDragObserverList = new List<Action>();
        if(m_happenHandlingObserverQueue == null)
            m_happenHandlingObserverQueue = new Queue<Action>();
        m_canDragSign.SetActive(false);

        m_cards = new List<Card>();
        m_mode = MODE.SINGLE;
    }
    private void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        m_canDrag = false;
    }
    private void Update()
    {
    }

    //ドラッグの処理
    List<Card> m_virtualCards = new List<Card>();
    List<Action> m_beginDragObserverList;
    List<Action> m_endDragObserverList;
    static Queue<Action> m_happenHandlingObserverQueue;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_canDrag) return;
        //半透明のカードを複製
        Card virtualCard = Instantiate(this, GameManager.Instance.m_TopLayerCanvas.transform);
        Color originColor = m_attachedObject.color;
        virtualCard.m_attachedObject.color = new Color(originColor.r, originColor.g, originColor.b, 0.5f);
        m_virtualCards.Add(virtualCard);
        foreach(Card subCard in m_cards)
        {
            Card virtualSubCard = Instantiate(subCard, GameManager.Instance.m_TopLayerCanvas.transform);
            Color subOriginColor = m_attachedObject.color;
            virtualSubCard.m_attachedObject.color = new Color(subOriginColor.r, subOriginColor.g, subOriginColor.b, 0.5f);
            m_virtualCards.Add(virtualSubCard);
        }

        foreach (var action in m_beginDragObserverList)
        {
            action();
        }
    }

    RaycastHit2D[] hits; //カーソル上にあるオブジェクト
    public void OnDrag(PointerEventData eventData)
    {
        if (!m_canDrag) return;
        //Debug.Log("OnDrag");
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(eventData.position);
        targetPos.z = 0;
        foreach (Card c in m_virtualCards)
        {
            c.transform.position = targetPos;
        }
        m_isDragging = true;

        if (hits != null)
        {
            foreach (var hit in hits)
            {
                HoldCardObject cardDroppedFuncs = hit.collider.gameObject.GetComponent<HoldCardObject>();
                if (cardDroppedFuncs != null)
                    cardDroppedFuncs.CardNotHover();
            }
        }
        hits = Physics2D.RaycastAll(targetPos, new Vector3(0, 0, 1));
        foreach(var hit in hits)
        {
            HoldCardObject cardDroppedFuncs = hit.collider.gameObject.GetComponent<HoldCardObject>();
            if (cardDroppedFuncs != null)
                cardDroppedFuncs.CardHover();
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_canDrag) return;
        //Debug.Log("OnEndDrag");
        foreach(Card c in m_virtualCards)
        {
            if (c != null)
                Destroy(c.gameObject);
        }
        m_virtualCards.Clear();
        m_isDragging = false;

        // ドラップされたオブジェクトを検出
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(eventData.position);
        hits = Physics2D.RaycastAll(targetPos, new Vector3(0, 0, 1));

        foreach (var aciton in m_endDragObserverList) //endDragObserverを実行
        {
            aciton();
        }
        foreach (var hit in hits)
        {
            HoldCardObject cardDroppedFuncs = hit.collider.gameObject.GetComponent<HoldCardObject>();
            if (cardDroppedFuncs != null)
            {
                cardDroppedFuncs.CardDrop(this);

                //happenHandlingObserverを実行
                int count = m_happenHandlingObserverQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    m_happenHandlingObserverQueue.Dequeue()();
                }
            }
        }
    }

    public void RegistBeginDragObserver(Action func)
    {
        m_beginDragObserverList.Add(func);
    }
    public void RegistEndDragObserver(Action func)
    {
        m_endDragObserverList.Add(func);
    }
    public void ClearDragObserverList()
    {
        m_beginDragObserverList.Clear();
        m_endDragObserverList.Clear();
    }
    public static void EnqueueHappenHandlingObserver(Action func)
    {
        m_happenHandlingObserverQueue.Enqueue(func);
    }
    public static void ClearHappenHandlingObserver()
    {
        m_happenHandlingObserverQueue.Clear();
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
        //Debug.Log("Anim_TurnOver start");
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
        Quaternion accurateRotation = this.transform.rotation;
        accurateRotation.y = 0;
        this.transform.rotation = accurateRotation;

        //Debug.Log("Anim_TurnOver end");
        yield return true;
    }

    public IEnumerator<bool> Anim_StraightLineMove(Vector3 afterPosition, int frameToSpend = 20)
    {
        //Debug.Log("Anim_StraightLineMove start");

        Vector3 distOfFrame = (afterPosition - this.transform.position) / frameToSpend;
        for (int i = 0; i < frameToSpend; i++)
        {
            this.transform.position += distOfFrame;
            if(m_mode == MODE.COMBINED)
            {
                m_cards[0].transform.position += distOfFrame;
            }
            yield return false;
        }
        this.transform.position = afterPosition;
        if (m_mode == MODE.COMBINED)
        {
            m_cards[0].transform.position = afterPosition;
        }

        //Debug.Log("Anim_StraightLineMove end");
        yield return true;
    }

    public IEnumerator<bool> Anim_StraightLineMoveWithTurnOver(Vector3 afterPosition, int frameToSpend = 20)
    {
        IEnumerator<bool> lineMove = Anim_StraightLineMove(afterPosition, frameToSpend);
        IEnumerator<bool> turnOver = Anim_TurnOver(frameToSpend);

        while(!lineMove.Current || !turnOver.Current)
        {
            lineMove.MoveNext();
            turnOver.MoveNext();
            yield return false;
        }
        yield return true;
    }

    public IEnumerator<bool> Anim_Rotate(float afterRotation, int frameToSpend = 20)
    {
        float rotateAnglePerFrame = (afterRotation - this.transform.rotation.z) / frameToSpend;
        for(int i = 0; i < frameToSpend; i++)
        {
            this.transform.Rotate(0,0,rotateAnglePerFrame);
            yield return false;
        }

        this.transform.eulerAngles = new Vector3(0, 0, afterRotation);
        yield return true;
    }

    public IEnumerator<bool> Anim_StraightLineMoveWithRotate(Vector3 afterPosition, float afterRotation, int frameToSpend = 20)
    {
        IEnumerator<bool> lineMove = Anim_StraightLineMove(afterPosition, frameToSpend);
        IEnumerator<bool> rotate = Anim_Rotate(afterRotation, frameToSpend);

        while (!lineMove.Current && !rotate.Current)
        {
            if(lineMove.Current == false)
            {
                lineMove.MoveNext();
            }
            if (rotate.Current == false)
            {
                rotate.MoveNext();
            }
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

    public override void AddCard(Card card, bool doAnim = true)
    {
        Debug.Assert(m_mode == MODE.WAIT_COMBINE || m_mode == MODE.WAIT_COMBINE);

        Transform preCanvas = card.transform.parent; // Canvas <- Card


        m_cards.Add(card);
        card.transform.SetAsLastSibling(); // 描写を一番最後に設定

        //Combine
        if (m_mode == MODE.WAIT_COMBINE)
        {
            Debug.Assert(m_cards.Count == 1);

            m_num = (this.m_num + m_cards[0].m_num) % 13;
            m_mode = MODE.COMBINED;
            card.SetMode(MODE.CONTAINED);

            AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[0].Anim_StraightLineMoveWithRotate(this.transform.position, this.transform.rotation.z + 90));
        }

        //Compress
        if (m_mode == MODE.WAIT_COMPRESS)
        {
            throw new System.NotImplementedException();
            Debug.Assert(m_cards.Count > 1);
            //m_mode = MODE.COMPRESSED;

            const float gap = 10;
            Vector3 pos0 = this.transform.position;
            Vector3 pos1 = this.transform.position;
            pos0.y -= gap;
            pos1.y += gap;
            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[0].Anim_StraightLineMove(pos0));
            AnimationQueue.Instance.AddAnimToLastIndex(m_cards[1].Anim_StraightLineMove(pos1));
        }

        //Hand内の移動は重ね終わった後にやる
        if (preCanvas != null)
        {
            Transform parent = preCanvas.parent; // HoldCardObject <- Canvas
            //Debug.Log(card.name + "'s parent is " + parent.name);
            AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
            parent.GetComponent<HoldCardObject>().RemoveCard(card, true);
        }

    }

    public bool IsContinuous(Card other)
    {
        if(this.m_suit == Suit.Joker || other.m_suit == Suit.Joker) 
            return true;

        int big = this.m_num + 1;
        int small = this.m_num - 1;
        if (big == 14) big = 1;
        if (small == 0) small = 13;
        return other.m_num == big || other.m_num == small;
    }

    public bool CanCompress(Card other)
    {
        return this.m_num == other.m_num;
    }

    public void EnableDrag()
    {
        this.m_canDrag = true;
        m_canDragSign.SetActive(true);
        //Debug.Log(name + "'s drag is enabled");
    }
    public void DisableDrag()
    {
        this.m_canDrag = false;
        m_canDragSign.SetActive(false);
        //Debug.Log(name + "'s drag is disabled");
    }

    public HoldCardObject GetParentHoldCardObject()
    {
        return this.transform.parent.parent.GetComponent<HoldCardObject>();
    }

    public void SetMode(MODE mode)
    {
        m_mode = mode;
        if(m_mode == MODE.CONTAINED)
        {
            this.GetComponent<Image>().raycastTarget = false;
        }
        else
        {
            this.GetComponent<Image>().raycastTarget = true;
        }
    }

    override public void CardDrop(Card droppedCard)
    {
        base.CardDrop(droppedCard);
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        AddCard(droppedCard);
    }
}
