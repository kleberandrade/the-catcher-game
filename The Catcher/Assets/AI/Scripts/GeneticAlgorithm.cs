using System;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    public List<Chromosome> Population;
    public int Generation;
    public float BestFitness;

    public float MutationRate;

    private List<Chromosome> m_NewPopulation;
    private Random m_Random;

    public GeneticAlgorithm(int populationSize, Random random, int elitism, float mutationRate)
    {
        Generation = 1;
        MutationRate = mutationRate;
        Population = new List<Chromosome>(populationSize);
        m_NewPopulation = new List<Chromosome>(populationSize);
        m_Random = random;

        for (int i = 0; i < populationSize; i++)
            Population.Add(new Chromosome(new float[] {
                Helper.Map((float)m_Random.NextDouble(), 0.0f, 1.0f, 0.5f, 1.0f),
                Helper.Map((float)m_Random.NextDouble(), 0.0f, 1.0f, 0.0f, 0.2f) }));
    }

    public void NewGeneration()
    {
        Population.Sort(CompareChromosome);

        m_NewPopulation.Clear();

        m_NewPopulation.Add(Population[0]);

        for (int i = 1; i < Population.Count; i++)
        {
            Chromosome child = (Chromosome)m_NewPopulation[0].Clone();

            child.Genes[0] += Helper.Map((float)m_Random.NextDouble(), 0.0f, 1.0f, -MutationRate, MutationRate);
            child.Genes[1] += Helper.Map((float)m_Random.NextDouble(), 0.0f, 1.0f, -MutationRate, MutationRate);

            child.Genes[0] = Helper.Clamp(child.Genes[0], 0.0f, 1.0f);
            child.Genes[1] = Helper.Clamp(child.Genes[1], 0.0f, 1.0f);

            m_NewPopulation.Add(child);
        }

        Population = new List<Chromosome>(m_NewPopulation);

        Generation++;
    }

    private int CompareChromosome(Chromosome a, Chromosome b)
    {
        if (a.Fitness > b.Fitness)
            return -1;

        if (a.Fitness < b.Fitness)
            return 1;

        return 0;
    }
}