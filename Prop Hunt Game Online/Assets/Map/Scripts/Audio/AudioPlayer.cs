using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource PlayerAudioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip TransformationClip;

    public void PlayTransformaitonSound()
    {
        if (!PlayerAudioSource) return;

        PlayerAudioSource.clip = TransformationClip;
        PlayerAudioSource.Play();
    }
}
