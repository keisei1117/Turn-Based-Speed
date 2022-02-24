using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class Button : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
    }

    static Color enabledButtonColor { get; } = new Color(0.3f, 0.3f, 0.3f, 1);
    static Color enabledTextColor { get; } = new Color(1, 1, 1, 1);
    static Color disabledButtonColor { get; } = new Color(0.3f, 0.3f, 0.3f, 0.4f);
    static Color disabledTextColor { get; } = new Color(1, 1, 1, 0.4f);

    private Action m_ButtonPressedBehave;
    bool m_isEnabled;

    public void RegistPressedBehave(Action action)
    {
        m_ButtonPressedBehave = action;
    }
    public void ClearPressedBehave()
    {
        m_ButtonPressedBehave = null;
    }
    public void Pressed()
    {
        if (m_isEnabled == false) return;

        Debug.Log("TurnEndButtonClicked");
        if (m_ButtonPressedBehave != null)
            m_ButtonPressedBehave();
    }
    public void Enable()
    {
        m_isEnabled = true;
        this.GetComponent<Image>().color = enabledButtonColor;
        this.GetComponentInChildren<Text>().color = enabledTextColor;
    }
    public void Disable()
    {
        m_isEnabled = false;
        this.GetComponent<Image>().color = disabledButtonColor;
        this.GetComponentInChildren<Text>().color = disabledTextColor;
    }
}
