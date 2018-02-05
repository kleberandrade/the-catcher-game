using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MessageDialog : MonoBehaviour
{
    [Header("Dialog Components")]
    public RectTransform m_FaderPanel;
    public RectTransform m_MessagePanel;
    public Text m_Title;
    public Text m_Message;
    public Button[] m_Butons = new Button[2];
    public Text[] m_TextButtons = new Text[2];

    private CanvasGroup m_CanvasGroup;
    private bool m_IsShow;

    private float m_FadeTime = 0.1f;
    private bool m_IsFading = false;

    public bool IsShow
    {
        get { return m_IsShow; }
        set { m_IsShow = value; }
    }

    #region [ Singleton ]
    private static MessageDialog m_Instance = null;

    public static MessageDialog Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;
        m_FaderPanel.gameObject.SetActive(false);
        m_MessagePanel.gameObject.SetActive(false);
    }

    public void Show(string title, string message, string[] textButtons, UnityAction[] actionEvents)
    {
        Time.timeScale = 0.0f;

        m_IsShow = true;
        m_Title.text = title;
        m_Message.text = message;

        if (textButtons.Length == 0)
        {
            m_Butons[0].gameObject.SetActive(false);
            m_Butons[1].gameObject.SetActive(false);
        }
        else
        {
            m_Butons[0].gameObject.SetActive(true);
            m_Butons[0].onClick.RemoveAllListeners();
            m_Butons[0].onClick.AddListener(delegate { Hide(); });
            if (actionEvents[0] != null)
                m_Butons[0].onClick.AddListener(actionEvents[0]);
            m_TextButtons[0].text = textButtons[0].ToUpper();

            m_Butons[1].gameObject.SetActive(false);
            if (textButtons.Length == 2 && actionEvents.Length == 2)
            {
                m_Butons[1].gameObject.SetActive(true);
                m_Butons[1].onClick.RemoveAllListeners();
                m_Butons[1].onClick.AddListener(delegate { Hide(); });
                if (actionEvents[1] != null)
                    m_Butons[1].onClick.AddListener(actionEvents[1]);
                m_TextButtons[1].text = textButtons[1].ToUpper();
            }
        }

        m_FaderPanel.gameObject.SetActive(true);
        m_MessagePanel.gameObject.SetActive(true);
        StartCoroutine(Fade(true, new RectTransform[]{ m_FaderPanel, m_MessagePanel}));
    }

    public void Hide()
    {
        Time.timeScale = 1.0f;
        m_IsShow = false;
        StartCoroutine(Fade(false, new RectTransform[] { m_FaderPanel, m_MessagePanel }));
    }

    private IEnumerator Fade(bool fadeIn, RectTransform[] panels)
    {
        if (m_IsFading)
            yield break;

        m_IsFading = true;
        float rate = 1.0f / m_FadeTime;
        int startAlpha = 1 - Convert.ToInt32(fadeIn);
        int endAlpha = Convert.ToInt32(fadeIn);
        float progress = 0.0f;

        while (progress < 1.0)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            progress += rate * Time.fixedDeltaTime;

            yield return null;
        }

        if (!fadeIn)
        {
            foreach (RectTransform rect in panels)
                rect.gameObject.SetActive(false);
        }

        m_IsFading = false;
    }
}
