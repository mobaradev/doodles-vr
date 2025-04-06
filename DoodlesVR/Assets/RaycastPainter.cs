using UnityEngine;

public class RaycastPainter : MonoBehaviour
{
    public bool AllowPainting;
    [Header("Raycast Settings")]
    public float rayLength = 0.1f; // How far the ray should go
    public LayerMask paintableLayer;

    [Header("Brush Settings")]
    public Color brushColor = Color.red;
    public int brushSize = 8;

    [Header("Target Texture")]
    public Texture2D sourceTexture; // A white base texture
    public Texture2D runtimeTexture;

    private Renderer targetRenderer;

    private void Awake()
    {
        // Find the renderer on the paintable surface
        GameObject paintSurface = GameObject.FindGameObjectWithTag("PaintableSurface");
        //PlanePainter planePainter = paintSurface.GetComponent<PlanePainter>();
        //this.sourceTexture = planePainter.runtimeTexture;
        if (paintSurface == null)
        {
            Debug.LogError("No object with tag 'PaintableSurface' found in scene.");
            return;
        }

        targetRenderer = paintSurface.GetComponent<Renderer>();

        // Create a runtime texture copy
        runtimeTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(sourceTexture, runtimeTexture);
        runtimeTexture.Apply();
        this.ResetCanvas();

        // Apply the texture to the paintable material
        targetRenderer.material.mainTexture = runtimeTexture;
    }

    private void Update()
    {
        this.HandleOutput();

        if (!this.AllowPainting) return;

        GameObject paintSurface = GameObject.FindGameObjectWithTag("PaintableSurface");
        //PlanePainter planePainter = paintSurface.GetComponent<PlanePainter>();
        //this.sourceTexture = planePainter.runtimeTexture;
        //this.runtimeTexture = planePainter.runtimeTexture;
        Debug.DrawRay(transform.position, transform.forward * rayLength, Color.green);

        // Cast a short ray from this object's forward direction
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayLength, paintableLayer))
        {
            Debug.Log("Raycast");
            if (hit.collider.CompareTag("PaintableSurface"))
            {
                // Convert UV coords to pixel coords
                Vector2 uv = hit.textureCoord;
                int x = (int)(uv.x * runtimeTexture.width);
                int y = (int)(uv.y * runtimeTexture.height);

                PaintAt(x, y);
            }
        }
    }

    private void PaintAt(int x, int y)
    {
        Debug.Log("PaintAt x = " + x + ", y = " + y);
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

    void HandleOutput()
    {
        // rescale to 28x28
        //Texture2D resizedTexture = ResizeTexture(runtimeTexture, 28, 28);
        Texture2D resizedTexture = runtimeTexture;
        FindObjectOfType<ImageCl>().inputTexture = resizedTexture;

        //Sprite sprite = Sprite.Create(FindObjectOfType<ImageCl>().inputTexture,
        //                              new Rect(0, 0, 28, 28),
        //                              new Vector2(0.5f, 0.5f),
        //                              100f); // The pivot and pixels per unit
        //this.sUI.sprite = sprite;
        FindObjectOfType<ImageCl>().inputTexture = resizedTexture;
    }

    public void ResetCanvas()
    {
        for (int py = 0; py < 256; py++)
        {
            for (int px = 0; px < 256; px++)
            {
                runtimeTexture.SetPixel(px, py, Color.black);
            }
        }

        runtimeTexture.Apply();
    }

}
