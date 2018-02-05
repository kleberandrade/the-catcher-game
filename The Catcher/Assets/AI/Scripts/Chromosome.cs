using System;

[Serializable]
public class Chromosome : ICloneable
{
    public float[] Genes;

    public float Error;

    public float Fitness;

    public Chromosome(float[] genes)
    {
        Genes = new float[2];
        Array.Copy(genes, Genes, genes.Length);
        Error = 0.0f;
        Fitness = 0.0f;
    }

    public float Evaluate(AIParameter param)
    {
        Fitness = param.Kd * Distance + param.Ks * Speed - param.Ke * Error;
        return Fitness;
    }

    public float Distance
    {
        get { return Genes[0]; }
    }

    public float Speed
    {
        get { return Genes[1]; }
    }

    public object Clone()
    {
        var clone = new Chromosome(Genes);
        return clone;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}] - {2}", Genes[0], Genes[1], Fitness);
    }
}