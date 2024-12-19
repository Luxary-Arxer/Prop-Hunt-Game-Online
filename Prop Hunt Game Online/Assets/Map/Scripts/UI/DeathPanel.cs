using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanel : MonoBehaviour
{
    [Header("UI objects")]
    public GameObject DeathPanelObject;
    public GameObject AimImage;
    public GameObject PressAnyKeyImage;

    [Header("Character")]
    public Player player;

    [Header("Post processing")]
    [SerializeField] private ColorGrading _cameraColorGrading;
    [SerializeField] private Shader _deathRendererShader;

    [Header("Values")]
    [SerializeField] private float PressKeyDelay = 1;//Time after which you can restart the game
    private float _seconds;

    // Update is called once per frame
    void Update()
    {
        if(player.Health <= 0)
        {
            if (player.enabled)
                player.CheckIfDead();//"Player" component check if player is dead

            DeathPanelObject.SetActive(true);
            AimImage.SetActive(false);

            _cameraColorGrading.SetRendererShader(_deathRendererShader);

            _seconds += Time.deltaTime;
            if (_seconds > PressKeyDelay)
            {
                PressAnyKeyImage.SetActive(true);
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}
