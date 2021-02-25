using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public AudioClip audioClip;
    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) == true)
        {
            // Torigger
            Debug.Log("A!");
        }
    }
}