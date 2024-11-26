/*
 *   MoveNet Multipose
 *   Copyright © 2024 NatML Inc. All Rights Reserved.
 */

using UnityEngine;
using Function;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VideoKit;
using Function.Types;

[Function.Function.Embed(Tag)]
public class MoveNetMultiposeSample : MonoBehaviour {

    [Header(@"Prediction")]
    public VideoKitCameraManager cameraManager;
    public Texture2D image;
    public bool realtime;

    [Header(@"UI")]
    public PoseVisualizer visualizer;

    private Function.Function fxn;

    /// <summary>
    /// MoveNet Multipose predictor tag on Function.
    /// See https://fxn.ai/@natml/movenet-multipose
    /// </summary>
    private const string Tag = "@natml/movenet-multipose";

    private async void Start () {
        // Preload the predictor
        fxn = FunctionUnity.Create();
        await fxn.Predictions.Create(tag: Tag, inputs: new()); // Run this once
        // Listen for camera
        if (!realtime)
            DetectPoses(image);
        else {
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
            await cameraManager.StartRunningAsync();
        }
    }
    
    private void OnCameraFrame () => DetectPoses(cameraManager.texture);

    private void DetectPoses (Texture2D image) {
        // Predict pose
        Prediction prediction = fxn.Predictions.Create(
            tag: Tag,
            inputs: new () { ["image"] = image.ToImage() }
        ).Result;
        Pose[] poses = (prediction.results[0] as JArray).ToObject<Pose[]>(Pose.Serializer);
        // Visualize
        visualizer.Render(image, poses);
    }
}