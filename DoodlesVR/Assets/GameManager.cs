using System.Collections.Generic;
using MoreMountains.Feedbacks;
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

    public MMF_Player SuccessParticleFeedback;
    public MMF_Player WrongParticleFeedback;
    public RaycastPainter RaycastPainter;
    public List<GameObject> WrongEffects;
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
        this.PointsText.SetText(this.Points + " points");
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

        if (predictedClass.classId == this.ClassToDraw && predictedClass.percentage > 0.005)
        {
            this.OnCorrectDraw();
        } else
        {
            this.OnWrongDraw();
        }
    }

    public void Skip()
    {
        this.OnSkip();
    }

    public void OnCorrectDraw()
    {
        this.SuccessParticleFeedback?.PlayFeedbacks();
        this.Points += 1;
        this.RandomizeClassToDraw();
        this.RaycastPainter.ResetCanvas();
    }

    public void OnWrongDraw()
    {
        this.WrongParticleFeedback?.PlayFeedbacks();
        Instantiate(this.WrongEffects[Random.Range(0, this.WrongEffects.Count)], new Vector3(2.39f, 4.59f, 4.14f), Quaternion.identity);
    }

    public void OnSkip()
    {
        this.RandomizeClassToDraw();
        this.RaycastPainter.ResetCanvas();
    }
}
