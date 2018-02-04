using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Nut : MonoBehaviour 
{
    #region [ Events ]
    public delegate void CollisionAction(Nut nutScript);
    public static event CollisionAction OnGroundCollision;
    public static event CollisionAction OnBasketCollision;
    #endregion

    public AudioClip m_GroundAudioClip;
    public AudioClip m_CapturedAudioClip;
    public float m_Speed = 0.5f;

    public float m_TimeToDestroy = 1.0f;

    public ParticleSystem m_SpurtParticle;

    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private Collider m_Collider;
    private Rigidbody m_Rigidbody;

    private bool m_IsFalling = true;

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
    }

    private void Start()
    {
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        m_IsFalling = true;
    }

    private void FixedUpdate()
    {
        if (!m_IsFalling)
            return;

        m_Rigidbody.velocity = Vector3.down * m_Speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            GroundCollision();

        if (other.transform.CompareTag("Basket"))
            BacketCollision();
    }

    private void GroundCollision()
    {
        m_Collider.enabled = false;
        m_IsFalling = false;

        m_AudioSource.clip = m_GroundAudioClip;
        m_AudioSource.Play();

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_Animator.SetBool("InGround", true);

        Spurt();

        StartCoroutine(GroundDestroy());

        if (OnGroundCollision != null)
            OnGroundCollision(this);
    }

    private IEnumerator GroundDestroy()
    {
        yield return new WaitForSeconds(m_TimeToDestroy);
        m_Rigidbody.constraints = RigidbodyConstraints.None;
        Destroy(gameObject);
    }

    public void BacketCollision()
    {
        m_IsFalling = false;
        m_Collider.enabled = false;

        m_AudioSource.clip = m_CapturedAudioClip;
        m_AudioSource.Play();

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        Destroy(gameObject, m_TimeToDestroy);

        if (OnBasketCollision != null)
            OnBasketCollision(this);
    }

    private void Spurt()
    {
        ParticleSystem spurt = (ParticleSystem)Instantiate(m_SpurtParticle, m_Rigidbody.position, Quaternion.identity);
        Destroy(spurt.gameObject, spurt.main.duration * 2.0f);
    }
}
