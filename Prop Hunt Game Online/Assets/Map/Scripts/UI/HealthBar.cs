using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthBar : MonoBehaviour
{
    private Image _slider;

    [Header("Player")]
    public Player player;

    [Header("Slider parts")]
    public Image HeartImage;

    void Start()
    {
        _slider = GetComponent<Image>();
        //Set heart color to normal when game starts
        HeartImage.material.SetFloat("_GrayscaleAmount", 0);
    }

    void Update()
    {
        _slider.fillAmount = player.Health / 100;
        //Set heart color to gray when player die
        if(_slider.fillAmount == 0)
        HeartImage.material.SetFloat("_GrayscaleAmount", 1);
    }
}
