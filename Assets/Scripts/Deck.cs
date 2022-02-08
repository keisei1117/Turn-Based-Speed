using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public GameObject m_rangeObj;

    //m_cards[0]が一番下
    public List<Card> m_cards { get; private set; }

    private void Awake()
    {
        m_cards = new List<Card>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddCard(Card card)
    {
        card.transform.SetParent(this.transform.Find("Canvas").transform);
        m_cards.Add(card);
        card.transform.SetAsLastSibling();//描写を一番最後に設定
    }

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

    public void Shuffle()
    {
        Debug.Log(this.name + ": Shuffling");
        List<Card> ShuffledCards = new List<Card>();
        while (m_cards.Count != 0)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            int i = Random.Range(0, m_cards.Count);

            ShuffledCards.Add(m_cards[i]);
            m_cards.RemoveAt(i);
        }

        m_cards = ShuffledCards;

        AnimationQueue.Instance.CreateNewEmptyAnimListToEnd();
        AnimationQueue.Instance.AddAnimToLastIndex(Anim_Shuffle());
    }

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
        Debug.Log("Anim_Shuffle end");
        yield return true;
    }

    void SetViewOrder()
    {
        for (int i = 0; i < m_cards.Count; i++)
        {
            m_cards[i].transform.SetAsFirstSibling();
        }
    }
}
