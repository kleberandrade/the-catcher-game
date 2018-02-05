using System;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    public List<Chromosome> Population;
    public int Generation;
    public float BestFitness;
    public float[] BestGenes;

    public int Elitism;
    public float MutationRate;

    private List<Chromosome> m_NewPopulation;
    private Random m_Random;
    private float m_FitnessSum;
    private int m_ChromosomeSize;
    private Func<int, float> m_FitnessFunction;

    public GeneticAlgorithm(int populationSize, int chromosomeSize, Random random, Func<int, float> fitnessFunction, int elitism, float mutationRate = 0.01f)
    {
        Generation = 1;
        Elitism = elitism;
        MutationRate = mutationRate;
        Population = new List<Chromosome>(populationSize);
        m_NewPopulation = new List<Chromosome>(populationSize);
        m_Random = random;
        m_ChromosomeSize = chromosomeSize;
        m_FitnessFunction = fitnessFunction;

        BestGenes = new float[chromosomeSize];

        for (int i = 0; i < populationSize; i++)
            Population.Add(new Chromosome(chromosomeSize, random, fitnessFunction, shouldInitGenes: true));
    }

    public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
    {

    }

    private void CalculateFitness()
    {

    }

    private Chromosome ChooseParent()
    {
        return null;
    }
}
