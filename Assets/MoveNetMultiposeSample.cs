/*
*   MoveNet Multipose
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML.VideoKit;
    using NatML.Vision;
    using Visualizers;

    public class MoveNetMultiposeSample : MonoBehaviour {

        [Header(@"VideoKit")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public MoveNetMultiposeVisualizer visualizer;

        private MLModelData modelData;
        private MLModel model;
        private MoveNetMultiposePredictor predictor;

        private async void Start () {
            // Fetch the MoveNet Multipose model data
            modelData = await MLModelData.FromHub("@natml/movenet-multipose");
            // Create the model
            model = new MLEdgeModel(modelData);
            // Create the MoveNet Multipose predictor
            predictor = new MoveNetMultiposePredictor(model);
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict
            var feature = frame.feature;
            (feature.mean, feature.std) = modelData.normalization;
            var poses = predictor.Predict(feature);
            // Visualize
            visualizer.Render(poses);
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose model
            model?.Dispose();
        }
    }
}