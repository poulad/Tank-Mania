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
        GameManager.Current.Players[0] = new Player("First Player")
        {
            TankPrefab = TankPrefabs[0],
        };
        GameManager.Current.Players[1] = new Player("Guy 2")
        {
            TankPrefab = TankPrefabs[1],
        };
        GameManager.Current.Players[2] = new Player("Third")
        {
            TankPrefab = TankPrefabs[2],
        };
        GameManager.Current.Players[3] = new Player("Last Man")
        {
            TankPrefab = TankPrefabs[3],
        };

#if UNITY_EDITOR
        if (GameManager.ReturnToScene != null)
        {
            GameManager.Current.SwitchToScene(GameManager.ReturnToScene);
            return;
        }
#endif
        GameManager.Current.SwitchToLevelScene(1);
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
