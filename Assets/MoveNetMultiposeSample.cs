/*
 *   MoveNet Multipose
 *   Copyright © 2023 NatML Inc. All Rights Reserved.
 */

namespace NatML.Examples {

    using UnityEngine;
    using VideoKit;
    using Vision;
    using Visualizers;

    public class MoveNetMultiposeSample : MonoBehaviour {

        [Header(@"VideoKit")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public MoveNetMultiposeVisualizer visualizer;

        private MoveNetMultiposePredictor predictor;

        private async void Start () {
            // Create the MoveNet Multipose predictor
            predictor = await MoveNetMultiposePredictor.Create();
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict
            var poses = predictor.Predict(frame);
            // Visualize
            visualizer.Render(poses);
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose the predictor
            predictor?.Dispose();
        }
    }
}