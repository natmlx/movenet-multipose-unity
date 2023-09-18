/*
*   MoveNet Multipose
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples.Visualizers {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using VideoKit.UI;
    using NatML.Vision;

    /// <summary>
    /// MoveNet multi-pose visualizer.
    /// This visualizer uses visualizes the pose keypoints overlaid on a UI panel.
    /// </summary>
    [RequireComponent(typeof(VideoKitCameraView))]
    public sealed class MoveNetMultiposeVisualizer : MonoBehaviour {

        #region --Inspector--
        public Image bodyRect;
        public RectTransform keypoint;
        #endregion


        #region --Client API--
        /// <summary>
        /// Render detected poses.
        /// </summary>
        /// <param name="poses">Body poses to render.</param>
        public void Render (params MoveNetMultiposePredictor.Pose[] poses) {
            // Delete current
            foreach (var rect in currentRects)
                GameObject.Destroy(rect.gameObject);
            foreach (var keypoint in currentKeypoints)
                GameObject.Destroy(keypoint.gameObject);
            currentRects.Clear();
            currentKeypoints.Clear();
            // Visualize
            foreach (var pose in poses) {
                var poseUI = Instantiate(bodyRect, transform);
                poseUI.gameObject.SetActive(true);
                VisualizeRect(pose, poseUI);
                currentRects.Add(poseUI);
                foreach (var point in pose) {
                    var keypointUI = Instantiate(keypoint, transform);
                    keypointUI.gameObject.SetActive(true);
                    VisualizeAnchor(point, keypointUI);
                    currentKeypoints.Add(keypointUI);
                }
            }
        }
        #endregion


        #region --Operations--
        private readonly List<Image> currentRects = new List<Image>();
        private readonly List<RectTransform> currentKeypoints = new List<RectTransform>();

        private void VisualizeRect (MoveNetMultiposePredictor.Pose pose, Image prefab) {
            var rectTransform = prefab.transform as RectTransform;
            var imageTransform = transform as RectTransform;
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, pose.rect.size);
            rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, pose.rect.position);
        }

        private void VisualizeAnchor (Vector2 point, RectTransform anchor) {
            var imageTransform = transform as RectTransform;
            anchor.anchorMin = 0.5f * Vector2.one;
            anchor.anchorMax = 0.5f * Vector2.one;
            anchor.pivot = 0.5f * Vector2.one;
            anchor.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);
        }
        #endregion
    }
}