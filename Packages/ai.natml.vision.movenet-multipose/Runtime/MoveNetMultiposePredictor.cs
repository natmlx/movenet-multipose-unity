/* 
*   MoveNet Multipose
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Collections.Generic;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// MoveNet multi-pose predictor.
    /// </summary>
    public sealed partial class MoveNetMultiposePredictor : IMLPredictor<MoveNetMultiposePredictor.Pose[]> {

        #region --Client API--
        /// <summary>
        /// Create the MoveNet multi-pose predictor.
        /// </summary>
        /// <param name="model">MoveNet multi-pose ML model.</param>
        /// <param name="minScore">Minimum pose candidate score.</param>
        /// <param name="smoothing">Apply smoothing filter to detected points.</param>
        public MoveNetMultiposePredictor (MLModel model, float minScore = 0.3f, bool smoothing = true) {
            this.model = model as MLEdgeModel;
            this.minScore = minScore;
            this.filter = smoothing ? new OneEuroFilter(0.5f, 3f, 1f) : null;
        }

        /// <summary>
        /// Detect the body pose in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected body pose.</returns>
        public Pose[] Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"MoveNet multi-pose predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            if (!MLImageType.FromType(input.type))
                throw new ArgumentException(@"MoveNet multi-pose predictor expects an an array or image feature", nameof(inputs));     
            // Predict
            using var inputFeature = (input as IMLEdgeFeature).Create(model.inputs[0]);
            using var outputFeatures = model.Predict(inputFeature);
            // Filter
            var keypoints = new MLArrayFeature<float>(outputFeatures[0]); // (1,6,56)
            var keypointData = keypoints.ToArray();
            keypointData = filter?.Filter(keypointData) ?? keypointData;
            // Create poses
            var result = new List<Pose>();
            for (int i = 0, ilen = keypoints.shape[1], istride = keypoints.shape[2]; i < ilen; ++i) {
                var offset = i * istride;
                keypointData[offset + 55] = keypoints[0,i,55];
                var pose = new Pose(keypointData, offset);
                if (pose.score >= minScore)
                    result.Add(pose);
            }
            return result.ToArray();
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly float minScore;
        private readonly OneEuroFilter filter;

        void IDisposable.Dispose () { } // Not used
        #endregion
    }
}