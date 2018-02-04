using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FadeInOut))]
public class Gameover : MonoBehaviour
{
    public Button m_ButtonClose;

    private FadeInOut m_Fade;
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Fade = GetComponent<FadeInOut>();

        m_ButtonClose.onClick.RemoveAllListeners();
        m_ButtonClose.onClick.AddListener(delegate { Close(); });
    }

    public void Show()
    {
        m_Fade.Fade(true, 1.0f, 0.0f);
    }
    
    private void Close()
    {
        m_AudioSource.Play();
        Transition.LoadScene("MainMenu", Color.black, 0.25f);
    }
}
