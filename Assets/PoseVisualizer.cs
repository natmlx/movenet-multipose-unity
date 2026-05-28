/*
*   MoveNet Multipose
*   Copyright © 2026 NatML Inc. All Rights Reserved.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Pose = MoveNetMultipose.Pose;

/// <summary>
/// Pose visualizer.
/// This visualizer uses visualizes the pose keypoints overlaid on a UI panel.
/// </summary>
[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
public sealed class PoseVisualizer : MonoBehaviour {

    #region --Inspector--
    [FormerlySerializedAs(@"bodyRect")] public Image bodyRectPrefab;
    [FormerlySerializedAs(@"keypoint")] public RectTransform keypointPrefab;
    #endregion


    #region --Client API--
    /// <summary>
    /// Render detected poses.
    /// </summary>
    /// <param name="poses">Body poses to render.</param>
    public void Render(Texture texture, params Pose[] poses) {
        // Show texture
        var rawImage = GetComponent<RawImage>();
        var aspectFitter = GetComponent<AspectRatioFitter>();
        rawImage.texture = texture;
        aspectFitter.aspectRatio = (float)texture.width / texture.height;
        // Delete current
        foreach (var rect in currentRects)
            GameObject.Destroy(rect.gameObject);
        foreach (var keypoint in currentKeypoints)
            GameObject.Destroy(keypoint.gameObject);
        currentRects.Clear();
        currentKeypoints.Clear();
        // Visualize
        foreach (var pose in poses) {
            // Render bounding rect
            var poseUI = Instantiate(bodyRectPrefab, transform);
            poseUI.gameObject.SetActive(true);
            VisualizeRect(pose, poseUI);
            currentRects.Add(poseUI);
            // Render keypoints
            foreach (var kp in pose.keypoints) {
                var keypointUI = Instantiate(keypointPrefab, transform);
                keypointUI.gameObject.SetActive(true);
                VisualizeAnchor(kp.position, keypointUI);
                currentKeypoints.Add(keypointUI);
            }
        }
    }
    #endregion


    #region --Operations--
    private readonly List<Image> currentRects = new List<Image>();
    private readonly List<RectTransform> currentKeypoints = new List<RectTransform>();

    private void VisualizeRect(Pose pose, Image prefab) {
        var rectTransform = prefab.transform as RectTransform;
        var imageTransform = transform as RectTransform;
        rectTransform.anchorMin = 0.5f * Vector2.one;
        rectTransform.anchorMax = 0.5f * Vector2.one;
        rectTransform.pivot = Vector2.zero;
        rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, pose.rect.size);
        rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, pose.rect.position);
    }

    private void VisualizeAnchor(Vector2 point, RectTransform anchor) {
        var imageTransform = transform as RectTransform;
        anchor.anchorMin = 0.5f * Vector2.one;
        anchor.anchorMax = 0.5f * Vector2.one;
        anchor.pivot = 0.5f * Vector2.one;
        anchor.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);
    }
    #endregion
}