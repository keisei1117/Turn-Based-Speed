using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    [SerializeField] private Image m_image;
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
        if (m_cards.Count != 0)
        {
            m_image.sprite = m_cards[0].GetImage();
        }
        else
        {
            m_image.enabled = false;
        }
    }

    public void AddCard(Card card)
    {
        m_cards.Add(card);
    }

    public Card DrawCard()
    {
        Card c = m_cards[0];
        m_cards.RemoveAt(0);
        return c;
    }

    //現在のカードの順番をログに出力
    public void LogNowOrder()
    {
        string str = this.name + ": Deck's Order\n";
        for(int i = 0; i < m_cards.Count; i++)
        {
            str += "\tm_cards[" + i.ToString() + "]: ";
            str += m_cards[i].GetImage().name;
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

        AnimationManager.Instance.AddPlayingAnimation(Anim_Shuffle(), true);
    }

    IEnumerator<bool> Anim_Shuffle()
    {
        Image animCard1 = Instantiate(m_image, this.GetComponentInChildren<Canvas>().transform, false);
        Image animCard2 = Instantiate(m_image, this.GetComponentInChildren<Canvas>().transform, false);
        Image animCard3 = Instantiate(m_image, this.GetComponentInChildren<Canvas>().transform, false);
        animCard1.name = "animCard1";
        animCard2.name = "animCard2";
        animCard3.name = "animCard3";
        animCard1.enabled = true;
        animCard2.enabled = true;
        animCard3.enabled = true;


        const float distance = 0.2f;
        const int loopCount = 10;
        for (int i = 0; i < 3; i++)
        {
            // 表示における上下関係の設定
            animCard3.transform.SetAsFirstSibling();
            animCard1.transform.SetAsFirstSibling();
            animCard2.transform.SetAsFirstSibling();

            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(distance, 0f, 0f);
                animCard2.transform.Translate(-distance, 0f, 0f);
                yield return false;
            }

            // 表示における上下関係の設定
            animCard1.transform.SetAsFirstSibling();
            animCard2.transform.SetAsFirstSibling();
            animCard3.transform.SetAsFirstSibling();

            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(-distance, 0f, 0f);
                animCard2.transform.Translate(distance, 0f, 0f);
                yield return false;
            }
        }

        // XXX: 削除されない 
        Destroy(animCard1);
        Destroy(animCard2);
        Destroy(animCard3);

        Debug.Log("Anim_Shuffle end");
        yield return true;
    }
}
