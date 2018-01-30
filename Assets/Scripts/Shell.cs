using UnityEngine;

// ReSharper disable once CheckNamespace
public class Shell : MonoBehaviour
{
    private AudioSource _audioSource;

    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        Destroy(gameObject, 3);
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject, 2);
        _audioSource.Play();
    }
}
