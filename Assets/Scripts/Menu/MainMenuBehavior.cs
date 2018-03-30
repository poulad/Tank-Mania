using TankMania;
using UnityEditor;
using UnityEngine;

public class MainMenuBehavior : MonoBehaviour
{
    public GameObject TankPrefab;

    public void Play()
    {
        GameManager.Current.Players[0] = new Player("First Player")
        {
            TankPrefab = TankPrefab,
        };
        GameManager.Current.Players[1] = new Player("Guy 2")
        {
            TankPrefab = TankPrefab,
        };
        GameManager.Current.Players[2] = new Player("Third")
        {
            TankPrefab = TankPrefab,
        };
        GameManager.Current.Players[3] = new Player("Last Man")
        {
            TankPrefab = TankPrefab,
        };
        GameManager.Current.SwitchToScene(Constants.Scenes.Playground);
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
