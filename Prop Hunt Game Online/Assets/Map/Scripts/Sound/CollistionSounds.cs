using UnityEngine;

public class CollistionSounds : MonoBehaviour
{
    [SerializeField] private AudioSource SurfaceAudioSource;
    public AudioClip[] collisionClip;

    private void OnCollisionEnter(Collision collision)
    {
        if (SurfaceAudioSource.isPlaying)
            return;

        SurfaceAudioSource.volume = collision.impulse.magnitude * 0.05f;// Set the sound volume depending on the force of the hit
        SurfaceAudioSource.clip = collisionClip[Random.Range(0, collisionClip.Length - 1)];//Pick random clip from the array
        SurfaceAudioSource.pitch = Time.timeScale;//That's was made for addapting to time scale
        SurfaceAudioSource.Play();
    }
}
