using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    //public RectTransform m_image;
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

        AnimationManager.Instance.AddPlayingAnimation(ShuffleAnimation, true);
    }



    // XXX: 表示されない
    IEnumerator<bool> ShuffleAnimation()
    {
        Debug.Log("ShuffleAnimation start");
        yield return false;

        Image animCard1 = Instantiate(m_image, this.GetComponentInChildren<Canvas>().transform, false);
        Image animCard2 = Instantiate(m_image, this.GetComponentInChildren<Canvas>().transform, false);
        animCard1.name = "animCard1";
        animCard2.name = "animCard2";

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

            Debug.Log("ShuffleAnimation step1" + "  i:" + i);

            animCard1.transform.SetAsFirstSibling();
            animCard2.transform.SetAsFirstSibling();
            m_image.transform.SetAsFirstSibling();

            for (int j = 0; j < loopCount; j++)
            {
                animCard1.transform.Translate(-distance, 0f, 0f);
                animCard2.transform.Translate(distance, 0f, 0f);
                yield return false;
            }

            Debug.Log("ShuffleAnimation step2");

            m_image.transform.SetAsFirstSibling();
            animCard1.transform.SetAsFirstSibling();
            animCard2.transform.SetAsFirstSibling();
        }

        // XXX: 削除されない
        Destroy(animCard1);
        Destroy(animCard2);
        Debug.Log(animCard1);
        Debug.Log(animCard2);

        Debug.Log("ShuffleAnimation end");
        yield return true;
    }
}
