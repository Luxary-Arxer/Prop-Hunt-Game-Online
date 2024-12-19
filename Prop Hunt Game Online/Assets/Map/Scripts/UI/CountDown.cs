using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public EnemySpawn enemySpawn;
    public Animation hideAlert;
    [Header("Number")]
    public Image Number;
    public List<Sprite> numberSprites = new List<Sprite>();
    [Header("Sound")]
    private AudioSource hideAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        hideAudioSource = GetComponent<AudioSource>();
        StartCoroutine(StartCountDown());  
    }

    IEnumerator StartCountDown()
    {
        //Start alert animation
        yield return new WaitForSeconds(0.5f);
        hideAlert.Play();
        hideAudioSource.Play();
        yield return new WaitForSeconds(1.5f);
        hideAlert.gameObject.SetActive(false);//Disable alert when animation was played
        //Set active number image to true
        Number.gameObject.SetActive(true);
        for(int i = numberSprites.Count - 1; i >= 0; i--)
        {
            //Change sprite after one second
            Number.sprite = numberSprites[i];
            yield return new WaitForSeconds(1);
        }
        enemySpawn.SpawnEnemy();//Spawn enemy when time is over
        gameObject.SetActive(false);//Turn off count down when time is over
    }
}
