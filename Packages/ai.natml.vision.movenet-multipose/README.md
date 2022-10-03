# MoveNet Multipose
[MoveNet](https://blog.tensorflow.org/2021/05/next-generation-pose-detection-with-movenet-and-tensorflowjs.html) multiple body pose detection from Google MediaPipe.

## Installing MoveNet Multipose
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.movenet-multipose": "1.0.0"
  }
}
```

## Predicting Poses in an Image
First, create the MoveNet Multipose predictor:
```csharp
// Fetch the model data from NatML Hub
var modelData = await MLModelData.FromHub("@natml/movenet-multipose");
// Deserialize the model
var model = modelData.Deserialize();
// Create the MoveNet predictor
var predictor = new MoveNetMultiposePredictor(model);
```

Then create an input feature:
```csharp
// Create image feature
Texture2D image = ...;
var input = new MLImageFeature(image);
// Set the normalization
(input.mean, input.std) = modelData.normalization;
```

Finally, detect the body pose in an image:
```csharp
// Detect the body pose
MoveNetMultiposePredictor.Pose[] pose = predictor.Predict(input);
```

___

## Requirements
- Unity 2021.2+

## Quick Tips
- Join the [NatML community on Discord](https://hub.natml.ai/community).
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!