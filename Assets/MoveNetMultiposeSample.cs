/*
 *   MoveNet Multipose
 *   Copyright © 2024 NatML Inc. All Rights Reserved.
 */

using UnityEngine;
using Function;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using VideoKit;

[Function.Function.Embed(Tag)]
public class MoveNetMultiposeSample : MonoBehaviour {

    //[Header(@"Camera")]
    //public VideoKitCameraManager cameraManager;

    [Header(@"Texture")]
    public Texture2D image;

    [Header(@"UI")]
    public PoseVisualizer visualizer;

    private Function.Function fxn;
    private bool predictorLoaded;

    /// <summary>
    /// MoveNet Multipose predictor tag on Function.
    /// See https://fxn.ai/@natml/movenet-multipose
    /// </summary>
    private const string Tag = "@natml/movenet-multipose";

    private async void Start () {
        // Load the predictor
        fxn = FunctionUnity.Create();
        await fxn.Predictions.Create(tag: Tag); // Run this once
        predictorLoaded = true;
    }

    private void Update () {
        // Check
        if (!predictorLoaded)
            return;
        // Predict pose
        var prediction = fxn.Predictions.Create(
            tag: Tag,
            inputs: new () { ["image"] = image.ToImage() }
        ).Result;
        var poses = (prediction.results[0] as JArray).ToObject<Pose[]>(Pose.Serializer);
        // Visualize
        visualizer.Render(image, poses);
    }
}