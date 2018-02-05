using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDebug : MonoBehaviour
{
    public int m_Number = 20;

    private AIDirector m_AI;

	private void Start ()
    {
        m_AI = AIDirector.Instance;

        for (int g = 0; g < m_Number; g++)
        {
            Chromosome c = m_AI.GetNextChromosome();
            m_AI.Evaluate(Random.Range(0.0f, 1.0f));

            Debug.Log(c.ToString());
        }

        m_AI.Save("Kleber");
    }
}
