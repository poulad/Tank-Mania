using TankMania;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MainMenuBehavior : MonoBehaviour
{
    public GameObject[] TankPrefabs;

    public void Play()
    {
        for (int i = 0; i < TankPrefabs.Length; i++)
        {
            GameManager.Current.Players[i] = new Player(TankPrefabs[i]);
        }

#if UNITY_EDITOR
        if (GameManager.ReturnToScene != null)
        {
            string scene = GameManager.ReturnToScene;
            GameManager.ReturnToScene = null;
            GameManager.Current.SwitchToScene(scene);
            return;
        }
#endif
        GameManager.Current.SwitchToScene(Constants.Scenes.Scores);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
