/*
*   MoveNet Multipose
*   Copyright © 2026 NatML Inc. All Rights Reserved.
*/

using UnityEngine;
using Muna;
using VideoKit.UI;

public class MoveNetMultiposeCameraSample : MonoBehaviour {

    [Header(@"Prediction")]
    public VideoKitCameraView cameraView;
    [Range(0f, 1f)] public float minConfidence = 0.3f;
    public Acceleration acceleration;

    [Header(@"Visualization")]
    public PoseVisualizer visualizer;

    private Muna.Muna muna;
    private MoveNetMultipose movenet;
    private byte[] pixelData;

    private async void Start() {
        // Create the predictor
        muna = MunaUnity.Create();
        movenet = new MoveNetMultipose(muna);
        await movenet.Preload(acceleration);
        // Listen for camera frames
        cameraView.OnCameraFrame.AddListener(OnCameraFrame);
    }

    private void OnCameraFrame() {
        // Create prediction image
        pixelData ??= new byte[cameraView.texture.width * cameraView.texture.height * 4];
        var image = cameraView.texture.ToImage(pixelData);
        // Detect poses
        var poses = movenet.Predict(
            image,
            minConfidence: minConfidence,
            acceleration: acceleration
        ).Result;
        // Visualize
        visualizer.Render(cameraView.texture, poses);
    }
}