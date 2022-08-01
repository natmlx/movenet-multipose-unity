/*
*   MoveNet Multipose
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Features;
    using NatML.Vision;
    using Visualizers;

    public class MoveNetMultiposeSample : MonoBehaviour {

        [Header(@"UI")]
        public MoveNetMultiposeVisualizer visualizer;

        private CameraDevice cameraDevice;
        private TextureOutput previewTextureOutput;

        private MLModelData modelData;
        private MLModel model;
        private MoveNetMultiposePredictor predictor;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            // Fetch the MoveNet model
            Debug.Log("Fetching model from NatML...");
            modelData = await MLModelData.FromHub("@natml/movenet-multipose");
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the MoveNet predictor
            predictor = new MoveNetMultiposePredictor(model);
        }

        void Update () {
            // Check that the predictor has been created
            if (predictor == null)
                return;
            // Create the input feature
            var previewTexture = previewTextureOutput.texture;
            var inputFeature = new MLImageFeature(
                previewTexture.GetRawTextureData<byte>(),
                previewTexture.width,
                previewTexture.height
            );
            (inputFeature.mean, inputFeature.std) = modelData.normalization;
            // Detect
            var poses = predictor.Predict(inputFeature);
            // Visualize
            visualizer.Visualize(previewTexture, poses);
        }

        void OnDisable () {
            // Dispose model
            model?.Dispose();
        }
    }
}