using System.Collections;
using UnityEngine;

public class PlayerEyes : MonoBehaviour 
{
    public Texture[] m_EyesTexture;

    public float m_RepeatRate = 0.3f;

    [Range(0.0f, 1.0f)]
    public float m_Threshold = 0.7f;

    public Renderer m_Renderer;

	private void Start()
	{
        StartCoroutine(EyesChanged());
	}

    private IEnumerator EyesChanged()
	{
        yield return new WaitForSeconds(m_RepeatRate);
        float number = Random.Range(0.0f, 1.0f);
        m_Renderer.material.mainTexture = m_EyesTexture[number > m_Threshold ? 0 : 1];
        StartCoroutine(EyesChanged());
    }
}
