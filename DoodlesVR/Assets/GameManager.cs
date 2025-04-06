using TMPro;
using UnityEngine;
using static ClassifierController;

public class GameManager : MonoBehaviour
{
    public ImageCl ImageCl;
    public ClassifierController ClassifierController;
    public int ClassToDraw;

    public float Points;

    public TextMeshProUGUI ClassToDrawText;
    public TextMeshProUGUI PointsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Points = 0;
        this.RandomizeClassToDraw();
    }

    // Update is called once per frame
    void Update()
    {
        this.ClassToDrawText.SetText("Draw " + this.ImageCl.classes[this.ClassToDraw]);
        this.PointsText.SetText("Points " + this.Points);
    }

    public void RandomizeClassToDraw()
    {
        int numberOfClasses = this.ImageCl.classes.Count;
        this.ClassToDraw = Random.Range(0, numberOfClasses);
    }

    public void Submit()
    {
        this.ImageCl.Classify();
        PredictedClass predictedClass = this.ClassifierController.GetPredictedClass();

        if (predictedClass.classId == this.ClassToDraw)
        {
            this.RandomizeClassToDraw();
            this.Points += 1;
        }
    }

    public void Skip()
    {
        this.RandomizeClassToDraw();
    }

    public void OnCorrectDraw()
    {

    }

    public void OnWrongDraw()
    {

    }

    public void OnSkip()
    {

    }
}
