using UnityEditor;

public class WebGLBuilder
{
    public static void Build()
    {
        string[] scenes = {
            "Assets/Scenes/Main Menu.unity",
            "Assets/Scenes/Level 1.unity",
            "Assets/Scenes/Level 2.unity",
            "Assets/Scenes/Score Sheet.unity",
            "Assets/Scenes/Playground.unity",
        };

        string outputPath = "Build";

        BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.WebGL, BuildOptions.None);
    }
}
