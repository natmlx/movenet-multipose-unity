#nullable enable

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// See https://fxn.ai/@natml/movenet-multipose for the pose schema.
/// </summary>
public sealed class Pose {

    #region --Fields--
    public float score;
    public Rect rect;
    public Vector3 nose;
    public Vector3 leftEye;
    public Vector3 rightEye;
    public Vector3 leftEar;
    public Vector3 rightEar;
    public Vector3 leftShoulder;
    public Vector3 rightShoulder;
    public Vector3 leftElbow;
    public Vector3 rightElbow;
    public Vector3 leftWrist;
    public Vector3 rightWrist;
    public Vector3 leftHip;
    public Vector3 rightHip;
    public Vector3 leftKnee;
    public Vector3 rightKnee;
    public Vector3 leftAnkle;
    public Vector3 rightAnkle;
    #endregion


    #region --Serializer--
    /// <summary>
    /// The prediction function returns a JSON array, but we want to create fully-typed C# objects.
    /// To do so, we use a custom JsonSerializer that knows how to deserialize the JSON array into `Pose` objects.
    /// The serializer also helps us invert the Y coordinate of returned points, because in Unity +y foes from bottom to top.
    /// See https://fxn.ai/@natml/movenet-multipose for the pose schema.
    /// See https://www.newtonsoft.com/json/help/html/CustomJsonConverterGeneric.htm for creating custom converters.
    /// </summary>
    public static readonly JsonSerializer Serializer = CreateSerializer();

    private static JsonSerializer CreateSerializer () {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new Vector3Converter());
        serializer.Converters.Add(new RectConverter());
        return serializer;
    }
    #endregion


    #region --Converters--

    public sealed class RectConverter : JsonConverter {

        public override bool CanConvert (Type objectType) => objectType == typeof(Rect);

        public override object ReadJson (JsonReader reader, Type _, object? existing, JsonSerializer __) {
            var obj = JObject.Load(reader);
            var width = obj["width"]!.ToObject<float>();
            var height = obj["height"]!.ToObject<float>();
            var x = obj["x"]!.ToObject<float>();
            var y = 1f - obj["y"]!.ToObject<float>() - height;
            return new Rect(x, y, width, height);
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer _) {
            var rect = (Rect)value!;
            var obj = new JObject {
                ["x"] = rect.x,
                ["y"] = rect.y,
                ["width"] = rect.width,
                ["height"] = rect.height
            };
            obj.WriteTo(writer);
        }
    }

    public sealed class Vector3Converter : JsonConverter {
    
        public override bool CanConvert (Type objectType) => objectType == typeof(Vector3);

        public override object ReadJson (JsonReader reader, Type _, object? existing, JsonSerializer __) {
            var obj = JObject.Load(reader);
            return new Vector3(
                obj["x"]!.ToObject<float>(),
                1f - obj["y"]!.ToObject<float>(),
                obj["score"]!.ToObject<float>()
            );
        }

        public override void WriteJson (JsonWriter writer, object? value, JsonSerializer _) {
            var vector = (Vector3)value!;
            var obj = new JObject {
                ["x"] = vector.x,
                ["y"] = vector.y,
                ["score"] = vector.z
            };
            obj.WriteTo(writer);
        }
    }
    #endregion
}