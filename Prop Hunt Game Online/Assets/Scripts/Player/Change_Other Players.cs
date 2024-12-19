using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_OtherPlayers : MonoBehaviour
{
    [SerializeField] private GameObject currentModel; // El modelo actual del jugador
    [SerializeField] public bool Hunter = false;
    public GameObject CaraterMesh;
    public int PlayerProp_Id;
    public GameObject GunMesh;
    public SkinnedMeshRenderer Player_Renderer;
    public Material Material_Hunter, Material_Alien;
    // Start is called before the first frame update
    void Start()
    {
        PlayerTeam();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTeam();

        if (Hunter == true)
        {
            CaraterMesh.layer = 6;
            CaraterMesh.SetActive(true);
            currentModel.SetActive(false);
            //Shoot();

        }
        if (Hunter == false)
        {
            CaraterMesh.layer = 7;
        }
    }
    void PlayerTeam()
    {
        if (Hunter == false)
        {
            Material[] materials = new Material[Player_Renderer.sharedMaterials.Length];
            materials[0] = Material_Alien;
            Player_Renderer.sharedMaterials = materials;
            GunMesh.SetActive(false);
        }
        else
        {
            Material[] materials = new Material[Player_Renderer.sharedMaterials.Length];
            materials[0] = Material_Hunter;
            Player_Renderer.sharedMaterials = materials;
            GunMesh.SetActive(true);
        }
    }
}
