using System;

[Serializable]
public class AIParameter
{
    public float Kd;

    public float Ks;

    public float Ke;

    public int Elitism;

    public float MutationRate;

    public int PopulationSize;

    public int Seed;
}