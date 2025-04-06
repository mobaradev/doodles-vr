using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRPainter : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // Drag your Ray Interactor here
    public Texture2D sourceTexture;       // Original texture
    public Color brushColor = Color.red;
    public int brushSize = 10;

    private Texture2D runtimeTexture;
    private Renderer surfaceRenderer;

    void Start()
    {
        surfaceRenderer = GetComponent<Renderer>();

        runtimeTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(sourceTexture, runtimeTexture);
        runtimeTexture.Apply();

        surfaceRenderer.material.mainTexture = runtimeTexture;
    }

    void Update()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.CompareTag("PaintableSurface"))
            {
                Vector2 uv = hit.textureCoord;
                int x = (int)(uv.x * runtimeTexture.width);
                int y = (int)(uv.y * runtimeTexture.height);

                PaintAt(x, y);
            }
        }
    }

    void PaintAt(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                int px = x + i;
                int py = y + j;
                if (px >= 0 && px < runtimeTexture.width && py >= 0 && py < runtimeTexture.height)
                {
                    float dist = Mathf.Sqrt(i * i + j * j);
                    if (dist < brushSize)
                    {
                        runtimeTexture.SetPixel(px, py, brushColor);
                    }
                }
            }
        }
        runtimeTexture.Apply();
    }
}
