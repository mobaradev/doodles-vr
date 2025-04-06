using UnityEngine;
using UnityEngine.UI;

public class PlanePainter : MonoBehaviour
{
    public Camera cam;
    public Texture2D texture;
    public Color brushColor = Color.red;
    public int brushSize = 10;

    private Renderer rend;
    public Texture2D runtimeTexture;
    public Image sUI;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Create a copy of the texture so we can draw on it
        //runtimeTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        runtimeTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        //runtimeTexture = this.texture;

        
        Graphics.CopyTexture(texture, runtimeTexture);
        runtimeTexture.Apply();

        rend.material.mainTexture = runtimeTexture;

        for (int py = 0; py < 256; py++)
        {
            for (int px = 0; px < 256; px++)
            {
                runtimeTexture.SetPixel(px, py, Color.black);
            }
        }
        
        runtimeTexture.Apply();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 pixelUV = hit.textureCoord;
                Debug.Log("ZContact, x = " + pixelUV.x + ", y = " + pixelUV.y);
                pixelUV.x *= runtimeTexture.width;
                pixelUV.y *= runtimeTexture.height;

                DrawBrush((int)pixelUV.x, (int)pixelUV.y);
            }
        }

        this.HandleOutput();
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // Convert the world space contact point to local space relative to the painted surface
            Vector3 localContactPoint = transform.InverseTransformPoint(contact.point);

            // Convert the local position to texture coordinates (UV)
            Vector2 pixelUV = LocalToUV(localContactPoint);

            // Paint at the contact point
            PaintAtContact((int)pixelUV.x, (int)pixelUV.y);
        }
    }

    Vector2 LocalToUV(Vector3 localPos)
    {
        // Assuming the surface is a simple plane, you may need to adjust this if the surface is more complex.
        // Map local position to texture coordinates
        float u = Mathf.InverseLerp(-0.5f, 0.5f, localPos.x);  // Horizontal axis (X)
        float v = Mathf.InverseLerp(-0.5f, 0.5f, localPos.z);  // Vertical axis (Z)

        // Get the UV coordinates, and map them to the texture size
        u = Mathf.Clamp01(u) * runtimeTexture.width;
        v = Mathf.Clamp01(v) * runtimeTexture.height;

        return new Vector2(u, v);
    }

    void PaintAtContact(int x, int y)
    {
        // Iterate over the area surrounding the contact point to simulate brush size
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

        // Apply the updated texture to the surface
        runtimeTexture.Apply();
    }

    void HandleOutput()
    {
        // rescale to 28x28
        //Texture2D resizedTexture = ResizeTexture(runtimeTexture, 28, 28);
        Texture2D resizedTexture = runtimeTexture;
        FindObjectOfType<ImageCl>().inputTexture = resizedTexture;
        
        Sprite sprite = Sprite.Create(FindObjectOfType<ImageCl>().inputTexture,
                                      new Rect(0, 0, 28, 28),
                                      new Vector2(0.5f, 0.5f),
                                      100f); // The pivot and pixels per unit
        this.sUI.sprite = sprite;
        FindObjectOfType<ImageCl>().inputTexture = resizedTexture;
    }

    Texture2D ResizeTexture(Texture2D texture, int width, int height)
    {
        // Create a new texture with the desired dimensions
        Texture2D newTexture = new Texture2D(width, height);

        // Use Texture2D.Resize to scale the texture
        newTexture.Reinitialize(width, height);
        newTexture.SetPixels(texture.GetPixels(0, 0, texture.width, texture.height));
        newTexture.Apply();
        Debug.Log("New texture = " + newTexture.width);

        return newTexture;
    }

    void DrawBrush(int x, int y)
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
