using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FadeInOut))]
public class Gameover : MonoBehaviour
{
    public Button m_ButtonClose;

    public Text m_FinalScoreText;

    private FadeInOut m_Fade;
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Fade = GetComponent<FadeInOut>();

        m_ButtonClose.onClick.RemoveAllListeners();
        m_ButtonClose.onClick.AddListener(delegate { Close(); });

        gameObject.SetActive(false);
    }

    public void Show(float score)
    {
        m_FinalScoreText.text = string.Format("{0:0.0}%", score);
        m_Fade.Fade(true, 0.3f, 0.0f);
    }
    
    private void Close()
    {
        m_AudioSource.Play();

        AIDirector.Instance.Save(SessionManager.Instance.FileName);

        SessionManager.Instance.Save();

        Transition.LoadScene("MainMenu", Color.black, 2.0f);
    }
}
