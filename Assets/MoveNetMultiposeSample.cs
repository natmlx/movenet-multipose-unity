/*
 *   MoveNet Multipose
 *   Copyright © 2024 NatML Inc. All Rights Reserved.
 */

namespace NatML.Examples {

    using UnityEngine;
    using Function;
    using Function.Types;
    using Function.Types.Converters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    //using VideoKit;
    using Vision;
    using Visualizers;

    [Function.Embed(MoveNetMultiposeSample.Tag)]
    public class MoveNetMultiposeSample : MonoBehaviour {

        //[Header(@"Camera")]
        //public VideoKitCameraManager cameraManager;

        [Header(@"Texture")]
        public Texture2D image;

        [Header(@"UI")]
        public PoseVisualizer visualizer;

        /// <summary>
        /// MoveNet Multipose predictor tag on Function.
        /// See https://fxn.ai/@natml/movenet-multipose
        /// </summary>
        public const string Tag = "@natml/movenet-multipose";

        private async void Start () {
            // Load the predictor
            var fxn = FunctionUnity.Create();
            await fxn.Predictions.Create(tag: Tag); // Run this once
            // Predict pose // Run this per-frame
            var prediction = await fxn.Predictions.Create(
                tag: Tag,
                inputs: new () { ["image"] = image.ToImage() }
            );
            // Deserialize
            var serializer = CreateSerializer(); // cache this for continuous use
            var poses = (prediction.results[0] as JArray).ToObject<Pose[]>(serializer);
            // Visualize
            visualizer.Render(image, poses);
        }

        /// <summary>
        /// The prediction function returns a JSON array, but we want to create fully-typed C# objects.
        /// To do so, we use a custom JsonSerializer that knows how to deserialize the JSON array into `Pose` objects.
        /// The serializer also helps us invert the Y coordinate of returned points, because in Unity +y foes from bottom to top.
        /// See https://fxn.ai/@natml/movenet-multipose for the pose schema.
        /// See https://www.newtonsoft.com/json/help/html/CustomJsonConverterGeneric.htm for creating custom converters.
        /// </summary>
        /// <returns></returns>
        private static JsonSerializer CreateSerializer () {
            JsonSerializer serializer = new JsonSerializer();
            // Add `Vector2` converter that inverts the Y axis because in Unity, +y goes from bottom to top.
            serializer.Converters.Add(new Vector2Converter(vec => new Vector2(
                vec.x,
                1f - vec.y
            )));
            serializer.Converters.Add(new RectConverter()); // INCOMPLETE // This also needs to be vertically mirrored
            // Return
            return serializer;
        }

        /// <summary>
        /// See https://fxn.ai/@natml/movenet-multipose for the pose schema.
        /// </summary>
        public sealed class Pose {
            public float score;
            public Rect rect;
            public Vector2 nose;
            public Vector2 leftEye;
            public Vector2 rightEye;
            public Vector2 leftEar;
            public Vector2 rightEar;
            public Vector2 leftShoulder;
            public Vector2 rightShoulder;
            public Vector2 leftElbow;
            public Vector2 rightElbow;
            public Vector2 leftWrist;
            public Vector2 rightWrist;
            public Vector2 leftHip;
            public Vector2 rightHip;
            public Vector2 leftKnee;
            public Vector2 rightKnee;
            public Vector2 leftAnkle;
            public Vector2 rightAnkle;
        }
    }
}