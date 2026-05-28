/*
*   MoveNet Multipose
*   Copyright © 2026 NatML Inc. All Rights Reserved.
*/

using System;
using System.Threading.Tasks;
using UnityEngine;
using Muna;
using Newtonsoft.Json;

/// <summary>
/// MoveNet Multipose predictor.
/// This is self-contained, so you can freely copy and reuse it in your projects.
/// </summary>
[Muna.Muna.Embed(Tag)]
public sealed class MoveNetMultipose {

    #region --Client API--
    public struct Pose {
        /// <summary>
        /// Pose rect minimum x coordinate.
        /// </summary>
        public float x;
        /// <summary>
        /// Pose rect minimum y coordinate.
        /// </summary>
        public float y;
        /// <summary>
        /// Pose rect width.
        /// </summary>
        public float width;
        /// <summary>
        /// Pose rect height.
        /// </summary>
        public float height;
        /// <summary>
        /// Pose confidence score.
        /// </summary>
        public float confidence;
        /// <summary>
        /// Pose keypoints.
        /// </summary>
        public Keypoint[] keypoints;
        /// <summary>
        /// Pose bounding rectangle.
        /// </summary>
        [JsonIgnore]
        public readonly Rect rect => new(x, 1f- height - y, width, height);
    }

    /// <summary>
    /// Pose keypoint.
    /// </summary>
    public struct Keypoint {
        /// <summary>
        /// Normalized X position.
        /// </summary>
        public float x;
        /// <summary>
        /// Normalized Y position.
        /// </summary>
        public float y;
        /// <summary>
        /// Label.
        /// </summary>
        public string label;
        /// <summary>
        /// Confidence score.
        /// </summary>
        public float confidence;
        /// <summary>
        /// Position.
        /// </summary>
        [JsonIgnore]
        public readonly Vector2 position => new(x, 1f - y);
    }

    /// <summary>
    /// Create a MoveNet Multipose predictor.
    /// </summary>
    public MoveNetMultipose(Muna.Muna muna) => this.muna = muna;

    /// <summary>
    /// Preload the model.
    /// </summary>
    /// <param name="acceleration">Prediction acceleration.</param>
    public async Task Preload(Acceleration acceleration = Acceleration.LocalAuto) {
        await muna.Predictions.Create(
            tag: Tag,
            inputs: new(),
            acceleration: acceleration.AsString()
        );
    }

    /// <summary>
    /// Detect poses in an image.
    /// </summary>
    /// <param name="image">Input image.</param>
    /// <param name="minConfidence">Minimum confidence score.</param>
    /// <param name="acceleration">Prediction acceleration.</param>
    /// <returns>Detected poses.</returns>
    public async Task<Pose[]> Predict(
        Image image,
        float minConfidence = 0.3f,
        Acceleration acceleration = Acceleration.LocalAuto
    ) {
        var prediction = await muna.Predictions.Create(
            tag: Tag,
            inputs: new() {
                [@"image"] = image,
                [@"min_confidence"] = minConfidence
            },
            acceleration: acceleration.AsString()
        );
        if (!string.IsNullOrEmpty(prediction.error))
            throw new InvalidOperationException(prediction.error);
        var poses = ((Json)prediction.results[0]).ToObject<Pose[]>();
        return poses;
    }
    #endregion

    
    #region --Operations--
    private readonly Muna.Muna muna;
    private const string Tag = "@yusuf/movenet_multipose";
    #endregion
}