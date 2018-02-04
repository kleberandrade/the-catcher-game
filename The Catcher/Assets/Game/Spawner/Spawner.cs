using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject m_ObjectToSpawn;
    public float m_DelayToSpawning = 0.5f;

    private ParticleSystem m_Particles;
    private Transform m_Transform;
    private WindWord m_WindWorld;

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_Particles = GetComponent<ParticleSystem>();
        m_WindWorld = GetComponent<WindWord>();
    }

    public void Spawn(Vector3 position, float speed)
    {
        m_Transform.position = position;
        StartCoroutine(SpawningAtPosition(speed));
    }

    private IEnumerator SpawningAtPosition(float speed)
    {
        m_Particles.Play();
        m_WindWorld.Fan();

        yield return new WaitForSeconds(m_DelayToSpawning);

        GameObject go = Instantiate(m_ObjectToSpawn, m_Transform.position, Quaternion.identity) as GameObject;
        go.GetComponent<IObjectSpawned>().SetSpeed(speed);

    }
}


