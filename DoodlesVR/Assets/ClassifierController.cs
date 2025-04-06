using System;
using TMPro;
using UnityEngine;

public class ClassifierController : MonoBehaviour
{
    public ImageCl ImageCl;
    public TextMeshProUGUI PredictedClassText;

    public class PredictedClass
    {
        public string className = "";
        public float percentage = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PredictedClass predictedClass = this.GetPredictedClass();
        if (predictedClass.percentage > 0.15)
        {
            this.PredictedClassText.SetText(predictedClass.className + " (" + Math.Round(predictedClass.percentage, 2) + ")");
        } else
        {
            this.PredictedClassText.SetText("");
        }
    }

    public PredictedClass GetPredictedClass()
    {
        PredictedClass predictedClass = new PredictedClass();
        if (this.ImageCl.inputTexture)
        {
            predictedClass.percentage = this.ImageCl.highestValue;
            predictedClass.className = this.ImageCl.classes[this.ImageCl.highestId];
        }

        return predictedClass;
    }
}
