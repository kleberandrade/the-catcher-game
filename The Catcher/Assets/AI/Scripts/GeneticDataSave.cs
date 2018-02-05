using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GeneticDataSave
{
    public AIParameter Parameter;

    [NonSerialized]
    private  List<PopulationDataSave> m_Generations;

    public PopulationDataSave[] Generations;

    public GeneticDataSave(AIParameter param)
    {
        Parameter = param;
        m_Generations = new List<PopulationDataSave>();
    }

    public void AddPopulation(List<Chromosome> population)
    {
        m_Generations.Add(new PopulationDataSave(population));
    }

    public void Save(string folder, string fileName)
    {
        string filePath = "D:/" + folder;
        Directory.CreateDirectory(filePath);

        filePath += "/" + fileName + ".json";

        Generations = m_Generations.ToArray();

        string dataAsJson = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, dataAsJson);
    }
}

[Serializable]
public class PopulationDataSave
{
    public Chromosome[] Population;

    public PopulationDataSave(List<Chromosome> population)
    {
        Population = population.ToArray();
    }
}