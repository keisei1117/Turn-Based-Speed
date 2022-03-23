using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class Deck : HoldCardObject
{
    public bool m_canDrop;
    public bool m_isFront { get; protected set; }

    private void Awake()
    {
        m_cards = new List<Card>();
        m_canDrop = false;
        DisableReceiveDrop();
        m_isFront = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // カードを加える
    override public void AddCard(Card card, bool doAnim = true)
    {
        base.AddCard(card, doAnim);

        if (doAnim)
        {
            // アニメーション処理
            if (card.m_isFront == this.m_isFront)
            {
                AnimationQueue.Instance.AddAnimToLastIndex(card.Anim_StraightLineMove(this.gameObject.transform.position));
            }
            else
            {
                AnimationQueue.Instance.AddAnimToLastIndex(card.Anim_StraightLineMoveWithTurnOver(this.gameObject.transform.position));
            }
        }
    }

    // 一番上のCardを返す
    public Card GetTopCard()
    {
        int lastIndex = m_cards.Count - 1;
        return m_cards[lastIndex];
    }

    // カードを1枚引く
    // 戻り値に帰ったCardはこのDeckから消去される
    public Card DrawCard()
    {
        int lastIndex = m_cards.Count - 1;
        Card c = m_cards[lastIndex];
        m_cards.RemoveAt(lastIndex);
        
        return c;
    }

    //現在のカードの順番をログに出力
    public void LogNowOrder()
    {
        string str = this.name + ": Deck's Order\n";
        for(int i = 0; i < m_cards.Count; i++)
        {
            str += "\tm_cards[" + i.ToString() + "]: ";
            str += m_cards[i].GetComponent<Card>().GetImage().name;
            str += "\n";
        }
        Debug.Log(str);
    }

    static int randomSeed = 0;
    //シャッフルする　アニメーションも登録する
    public void Shuffle()
    {
        randomSeed++;
        //Debug.Log(this.name + ": Shuffling");
        //Debug.Log("Random seed is " + System.DateTime.Now.Millisecond * randomSeed);
        List<Card> ShuffledCards = new List<Card>();
        while (m_cards.Count != 0)
        {
            Random.InitState(System.DateTime.Now.Millisecond * randomSeed);
            int i = Random.Range(0, m_cards.Count);

            ShuffledCards.Add(m_cards[i]);
            m_cards.RemoveAt(i);
        }

        m_cards = ShuffledCards;

        AnimationQueue.Instance.AddAnimToLastIndex(Anim_Shuffle());
    }

    //シャッフルのアニメーションメソッド
    IEnumerator<bool> Anim_Shuffle()
    {
        if (m_cards.Count < 3) //TODO: 2枚の時のアニメーション
        {
            Debug.Log("can't shuffle");
            yield return true;
        }

        Card animCard1 = m_cards[m_cards.Count - 1];
        Card animCard2 = m_cards[m_cards.Count - 2];
        Card animCard3 = m_cards[m_cards.Count - 3];

        Transform topLayer = GameManager.Instance.m_TopLayerCanvas.transform;
        animCard1.transform.SetParent(topLayer);
        animCard2.transform.SetParent(topLayer);
        animCard3.transform.SetParent(topLayer);

        const float distance = 0.2f;
        const int loopCount = 10;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(distance, 0f, 0f);
                animCard2.transform.Translate(-distance, 0f, 0f);
                yield return false;
            }

            // 表示における上下関係の設定
            animCard1.transform.SetAsLastSibling();
            animCard3.transform.SetAsLastSibling();
            animCard2.transform.SetAsLastSibling();

            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(-distance, 0f, 0f);
                animCard2.transform.Translate(distance, 0f, 0f);
                yield return false;
            }

            animCard3.transform.SetAsLastSibling();
            animCard2.transform.SetAsLastSibling();
            animCard1.transform.SetAsLastSibling();
        }
        //Debug.Log("Anim_Shuffle end");

        Transform originLayer = this.transform.Find("Canvas");
        animCard1.transform.SetParent(originLayer);
        animCard2.transform.SetParent(originLayer);
        animCard3.transform.SetParent(originLayer);

        SetViewOrder();
        yield return true;
    }

    public void SetViewOrder()
    {
        for (int i = 0; i < m_cards.Count; i++)
        {
            m_cards[i].transform.SetAsLastSibling();
        }
    }

    override public void CardDrop(Card droppedCard)
    {
        base.CardDrop(droppedCard);
        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        this.AddCard(droppedCard);
    }
}
