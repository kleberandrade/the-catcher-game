using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Transform m_DebugTargetPosition;

    private Slider m_Slider;
    private Text m_Text;
    private FadeInOut m_Fade;

    private int m_CurrentTarget;

    private RectTransform m_RectTransform;
    private AudioSource m_AudioSource;

    private int Point;
    private int NumberOfTargets;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Slider = GetComponent<Slider>();
        m_Text = GetComponentInChildren<Text>();
        m_Fade = GetComponentInChildren<FadeInOut>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void SetNumberOfTargets(int numberOfTargets)
    {
        NumberOfTargets = numberOfTargets;

        m_Slider.maxValue = numberOfTargets;
        m_Slider.minValue = 0;

        Point = 0;
        m_CurrentTarget = 0;
        SetHealthUI();
    }

    public void NextPoint()
    {
        if (Point >= m_CurrentTarget)
            return;

        Point++;
        
        StartCoroutine(m_Fade.PulseInverse(0.2f));
        StartCoroutine(PlayAudio(0.2f));
        SetHealthUI();
    }

    public IEnumerator PlayAudio(float time)
    {
        yield return new WaitForSeconds(time);
        m_AudioSource.Play();
    }

    public void NextTarget()
    {
        m_CurrentTarget++;
        SetHealthUI();
    }

    private void SetHealthUI()
    {
        m_Slider.value = Mathf.Min(m_CurrentTarget, NumberOfTargets);
        m_Text.text = Point.ToString();
    }

    public Vector3 WorldPoint(Vector3 position)
    {
        Vector3 screenPoint = new Vector3(m_RectTransform.transform.position.x, m_RectTransform.transform.position.y, Helper.GetDepht(position));
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        return worldPoint;
    }

    public void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
            return;

        if (m_DebugTargetPosition)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Gizmos.DrawSphere(m_DebugTargetPosition.position, 1.0f);
            Gizmos.DrawSphere(WorldPoint(m_DebugTargetPosition.position), 1.0f);
        }
    }
}