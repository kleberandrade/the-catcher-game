using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(FadeInOut))]
public class TextHint : MonoBehaviour
{
    private Text m_Text;
    private FadeInOut m_Fade;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_Text = GetComponentInChildren<Text>();
        m_Fade = GetComponent<FadeInOut>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Pulse(string text, float hideTime)
    {
        StartCoroutine(Writing(text, hideTime, 0.0f));
        StartCoroutine(Pulsing(text, hideTime));
    }

    private IEnumerator Pulsing(string text, float hideTime)
    {
        yield return StartCoroutine(m_Fade.Fading(true, 1.0f, 0.0f));
        yield return new WaitForSeconds(hideTime);
        yield return StartCoroutine(m_Fade.Fading(false, 1.0f, 0.0f));
        m_Text.text = string.Empty;
    }

    public void Show(string text, float time, float delay)
    {
        m_Text.text = text;
        m_Fade.Fade(true, time, delay);
    }

    public void Show(string text, float time)
    {
        Show(text, time, 0.0f);
    }

    public void Show(string text)
    {
        Show(text, 1.0f, 0.0f);
    }

    public void Hide()
    {
        Hide(0.0f, 0.0f);
    }

    public void Hide(float time)
    {
        Hide(time, 0.0f);
    }

    public void Hide(float time, float delay)
    {
        m_Fade.Fade(false, time, delay);
    }

    public IEnumerator Writing(string text, float time, float delay) 
    {
        yield return new WaitForSeconds(delay + 0.05f * time);
        float timeToLetter = m_AudioSource.clip.length / (float)text.Length;
        m_AudioSource.Play();
        for (int index = 0; index < text.Length; index++) {
            m_Text.text = text.Substring(0, index);
            yield return new WaitForSeconds(timeToLetter);
        }
    }
}
