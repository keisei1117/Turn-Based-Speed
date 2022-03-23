using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject DrawButtonObj;
    public GameObject DiscardButtonObj;
    public GameObject CombineButtonObj;
    public GameObject CompressButtonObj;
    public GameObject TurnEndButtonObj;
    public Button DrawButton;
    public Button DiscardButton;
    public Button CombineButton;
    public Button CompressButton;
    public Button TurnEndButton;

    public GameObject TransitionObj;
    Animator transitionAnimator;
    public GameObject ResultObj;

    public Text m_myOrOppoTurn;


    private void Awake()
    {
        DrawButton = DrawButtonObj.GetComponent<Button>();
        DiscardButton = DiscardButtonObj.GetComponent<Button>();
        CombineButton = CombineButtonObj.GetComponent<Button>();
        CompressButton = CompressButtonObj.GetComponent<Button>();
        TurnEndButton = TurnEndButtonObj.GetComponent<Button>();

        DrawButton.Disable();
        DiscardButton.Disable();
        CombineButton.Disable();
        CompressButton.Disable();
        TurnEndButton.Disable();

        transitionAnimator = TransitionObj.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_myOrOppoTurn.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        m_myOrOppoTurn.enabled = GameManager.Instance.m_gameStatus.m_nowMode == GameStatus.Mode.PLAYING;

        if (GameManager.Instance.m_gameStatus.IsMyTurn())
        {
            m_myOrOppoTurn.text = "Your Turn";
        }
        else
        {
            m_myOrOppoTurn.text = "Opponent's Turn";
        }
    }
    
    public IEnumerator<bool> Anim_Transition(string message)
    {
        TransitionObj.transform.Find("Text").GetComponent<Text>().text = message;
        transitionAnimator.SetBool("active", true);

        int loss = 0;

        //Debug.Log("normlizedTime" + transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        while(!transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("TransitionAnimation")){
            loss++;
            yield return false;
        }

        //Debug.Log("loss frame: " + loss);
        //Debug.Log("normlizedTime" + transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        while (transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            //Debug.Log("normlizedTime" + transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return false;
        }
        transitionAnimator.SetBool("active", false);
        yield return true;
    }

    public void ShowResult(bool isPlayerWinner)
    {
        ResultObj.SetActive(true);
        if(isPlayerWinner)
        {
            ResultObj.transform.Find("Text").GetComponent<Text>().text = "You Win";
            ResultObj.transform.Find("RedLine").gameObject.SetActive(true);
            ResultObj.transform.Find("BlueLine").gameObject.SetActive(false);
        }
        else
        {
            ResultObj.transform.Find("Text").GetComponent<Text>().text = "You Lose";
            ResultObj.transform.Find("RedLine").gameObject.SetActive(false);
            ResultObj.transform.Find("BlueLine").gameObject.SetActive(true);
        }
    }
}
