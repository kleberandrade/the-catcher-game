using System;

[Serializable]
public class Chromosome : IComparable<Chromosome>, ICloneable
{
    public float[] Genes;

    public float Fitness;

    private Random m_Random;

    private Func<int, float> m_FitnessFunction;

    public Chromosome(int size, Random random, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
    {
        Genes = new float[size];
        m_Random = random;
        m_FitnessFunction = fitnessFunction;

        if (shouldInitGenes)
        {
            for (int i = 0; i < Genes.Length; i++)
                Genes[i] = (float)m_Random.NextDouble();
        }
    }

    public float CalculateFitness(int index)
    {
        Fitness = m_FitnessFunction(index);
        return Fitness;
    }

    public Chromosome Crossover(Chromosome otherParent)
    {
        Chromosome child = new Chromosome(otherParent.Genes.Length, m_Random, m_FitnessFunction);
        child.Genes = (float[])otherParent.Genes.Clone();
        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            Genes[i] += Helper.Map((float)m_Random.NextDouble(), 0.0f, 1.0f, -0.1f, 0.1f);
            Genes[i] = Helper.Clamp(Genes[i], 0.0f, 1.0f);
        }
    }

    public object Clone()
    {
        var clone = (Chromosome)MemberwiseClone();
        Array.Copy(Genes, clone.Genes, Genes.Length);
        return clone;
    }

    public int CompareTo(Chromosome other)
    {
        return Fitness.CompareTo(other.Fitness);
    }
}