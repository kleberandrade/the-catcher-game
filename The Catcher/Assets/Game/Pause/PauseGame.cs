using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PauseGame : MonoBehaviour
{
    #region [ Singleton ]
    private static PauseGame m_Instance = null;

    public static PauseGame Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);
    }
    #endregion

    public RectTransform m_PausePanel;

    public Button m_ResumeButton;

    public Button m_QuitButton;

    public string m_SceneQuitButton = "MainMenu";

    public KeyCode m_KeyCodeToTooglePause = KeyCode.Escape;

    public float m_FadeTime = 0.1f;

    private CanvasGroup m_CanvasGroup;
    private AudioSource m_AudioSource;

    public bool IsPaused { get; private set; }

    private bool m_IsFading = false;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();

        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;

        m_QuitButton.onClick.RemoveAllListeners();
        m_QuitButton.onClick.AddListener(delegate { Close(); });

        m_ResumeButton.onClick.RemoveAllListeners();
        m_ResumeButton.onClick.AddListener(delegate { TogglePause(); });

        IsPaused = false;
        m_IsFading = false;
        
        m_PausePanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_KeyCodeToTooglePause))
            TogglePause();
    }

    private void TogglePause()
    {
        if (!IsPaused)
            Show();
        else
            Hide();

        IsPaused = !IsPaused;
    }

    public void Show()
    {
        Time.timeScale = 0.0f;

        m_AudioSource.Play();
        m_PausePanel.gameObject.SetActive(true);
        StartCoroutine(Fade(true, m_PausePanel));
    }

    public void Hide()
    {
        Time.timeScale = 1.0f;

        m_AudioSource.Play();
        StartCoroutine(Fade(false, m_PausePanel ));
    }

    public void Close()
    {
        TogglePause();
        Transition.LoadScene(m_SceneQuitButton, Color.black, 2.0f);
    }

    private IEnumerator Fade(bool fadeIn, RectTransform panel)
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
            panel.gameObject.SetActive(false);

        m_IsFading = false;
    }
}
