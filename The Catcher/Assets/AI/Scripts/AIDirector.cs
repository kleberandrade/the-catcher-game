using System.IO;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    #region [ Singleton ]
    private static AIDirector m_Instance = null;

    public static AIDirector Instance
    {
        get { return m_Instance; }
    }

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);
    }
    #endregion

    public AIParameter m_Parameter;

    public string m_Folder = "AI Save";

    private System.Random m_Random = null;

    private GeneticAlgorithm m_GA;

    private GeneticDataSave m_Data;

    private int m_IndexChromosome = -1;

    private void Start()
    {
        m_Random = new System.Random(m_Parameter.Seed);

        m_Data = new GeneticDataSave(m_Parameter);

        m_GA = new GeneticAlgorithm(
            m_Parameter.PopulationSize,
            m_Random,
            m_Parameter.Elitism,
            m_Parameter.MutationRate
        );

        m_IndexChromosome = -1;
    }

    public Chromosome GetNextChromosome()
    {
        m_IndexChromosome++;

        CheckNextGeneration();

        return m_GA.Population[m_IndexChromosome];
    }

    public void Evaluate(float error)
    {
        m_GA.Population[m_IndexChromosome].Error = error;
        m_GA.Population[m_IndexChromosome].Evaluate(m_Parameter);
    }

    public void CheckNextGeneration()
    {
        if (m_IndexChromosome == m_Parameter.PopulationSize)
        {
            m_GA.NewGeneration();

            m_Data.AddPopulation(m_GA.Population);

            m_IndexChromosome = 0;
        }
    }

    public void Save(string fileName)
    {
        m_Data.Save(m_Folder, fileName);
    }
}
