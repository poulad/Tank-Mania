using UnityEditor;

public class WebGLBuilder
{
    public static void Build()
    {
        string[] scenes = {
            "Assets/Scenes/Main Menu.unity",
            "Assets/Scenes/Playground.unity",
            "Assets/Scenes/Score Sheet.unity",
        };

        string outputPath = "Build";

        BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.WebGL, BuildOptions.None);
    }
}
