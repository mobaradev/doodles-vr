using UnityEngine;
using Unity.Sentis;

public class ImageCl : MonoBehaviour
{
    public Texture2D inputTexture;
    public ModelAsset modelAsset;

    Model runtimeModel;
    Worker worker;
    public float[] results;
    public int highestId;
    public float highestValue;
    public int oImageW;
    public int oImageH;

    void Start()
    {
        //this.Classify();
        InvokeRepeating(nameof(this.Classify), 1f, 1f);
    }

    private void Update()
    {
        if (this.inputTexture)
        {
            this.oImageW = this.inputTexture.width;
            this.oImageH = this.inputTexture.height;
        }
    }

    public void Classify()
    {
        if (!this.inputTexture) return;
        Model sourceModel = ModelLoader.Load(modelAsset);

        // Create a functional graph that runs the input model and then applies softmax to the output.
        FunctionalGraph graph = new FunctionalGraph();
        FunctionalTensor[] inputs = graph.AddInputs(sourceModel);
        FunctionalTensor[] outputs = Functional.Forward(sourceModel, inputs);
        FunctionalTensor softmax = Functional.Softmax(outputs[0]);

        // Create a model with softmax by compiling the functional graph.
        runtimeModel = graph.Compile(softmax);

        // Create input data as a tensor
        Tensor inputTensor = TextureConverter.ToTensor(inputTexture, width: 28, height: 28, channels: 1);

        //using Tensor transposedInput = inputTensor.Transpose(new[] { 0, 2, 3, 1 }); // (1,28,28,1)
        //Tensor transposedInput = inputTensor.Transpose(1, 2); // Swap dimensions 1 (height) and 2 (width)
        //inputTensor = inputTensor.Transpose(new[] { 0, 2, 3, 1 }); // Now (1, 28, 28, 1)

        // Create an engine
        worker = new Worker(runtimeModel, BackendType.GPUCompute);

        // Run the model with the input data
        worker.Schedule(inputTensor);

        // Get the result
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;

        // outputTensor is still pending
        // Either read back the results asynchronously or do a blocking download call
        results = outputTensor.DownloadToArray();
        float value = 0;
        int currentIndex = 0;
        value = results[0];
        for (int i = 1; i < results.Length; i++)
        {
            if (results[i] > value)
            {
                value = results[i];
                currentIndex = i;
            }
        }
        this.highestId = currentIndex;
        this.highestValue = value;
    }

    void OnDisable()
    {
        // Tell the GPU we're finished with the memory the engine used
        worker.Dispose();
    }
}
