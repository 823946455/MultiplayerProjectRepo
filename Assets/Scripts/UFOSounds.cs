using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UFOSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _ufoShieldSoundSource;
    [SerializeField] private AudioClip _ufoShieldSoundClip;
    // Start is called before the first frame update
    
    void OnCollisionEnter(Collision collision)
    {
        _ufoShieldSoundSource.PlayOneShot(_ufoShieldSoundClip);
    }
}
