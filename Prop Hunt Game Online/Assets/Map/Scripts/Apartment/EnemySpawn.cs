using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject EnemyPrefab;
    public Transform SpawnPoint;
    [Header("Doors")]
    public DoorAnimation Door;
    [Header("Player")]
    public CameraHandler PlayerCameraHandler;
    public void SpawnEnemy()
    {
        //Open doors
        StartCoroutine(Door.OpenDoor());
        //Spawn enemy
        GameObject enemy = Instantiate(EnemyPrefab, SpawnPoint.transform.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().CameraHandler = PlayerCameraHandler;
    }
}
