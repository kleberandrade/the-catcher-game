using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public AIParameter m_Parameter;

    private void InitializePopulation()
    {

    }

	private void NextPopulation()
    {

    }

    public void Evaluate(float error)
    {

    }
}

[Serializable]
public class AIParameter
{
    public float Kd;

    public float Ks;

    public float Ke;

    public float MutationRate;

    public int PopulationSize;
}

