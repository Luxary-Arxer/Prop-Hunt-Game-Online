using UnityEngine;

public class ColorGrading : MonoBehaviour
{
    [SerializeField] private Shader _rendererShader;
    private Material _rendererMaterial;

    void Start()
    {
        SetRendererShader(_rendererShader);
    }

    public void SetRendererShader(Shader rendererShader) 
    {
        if (!rendererShader) return;

        _rendererShader = rendererShader;
        _rendererMaterial = new Material(_rendererShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_rendererMaterial)
            Graphics.Blit(source, destination, _rendererMaterial);
        else
            Graphics.Blit(source, destination);
    }
}
