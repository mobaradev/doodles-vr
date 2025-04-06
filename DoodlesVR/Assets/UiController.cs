using UnityEngine;

public class UiController : MonoBehaviour
{
    public RaycastPainter RaycastPainter;
    public ImageCl ImageCl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCanvas()
    {
        this.RaycastPainter.ResetCanvas();
    }

    public void Submit()
    {
        this.ImageCl.Classify();
    }
}
