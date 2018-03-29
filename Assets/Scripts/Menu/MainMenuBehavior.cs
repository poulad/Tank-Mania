using TankMania;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{
    public GameObject TankPrefab;

    public void Play()
    {
        GameObject[] tanks;
        {// ToDo Testing
            tanks = new[]
            {
                Instantiate(TankPrefab, new Vector3(-4, 2, 0), Quaternion.identity, GameState.Current.transform),
                Instantiate(TankPrefab, new Vector3(0, 2, 0), Quaternion.identity, GameState.Current.transform),
                Instantiate(TankPrefab, new Vector3(3, 2, 0), Quaternion.identity, GameState.Current.transform),
                Instantiate(TankPrefab, new Vector3(6, 2, 0), Quaternion.identity, GameState.Current.transform),
            };

            foreach (var sr in tanks[1].GetComponentsInChildren<SpriteRenderer>())
                sr.color = Color.red;

            foreach (var sr in tanks[2].GetComponentsInChildren<SpriteRenderer>())
                sr.color = Color.blue;

            foreach (var sr in tanks[3].GetComponentsInChildren<SpriteRenderer>())
                sr.color = Color.gray;
        }

        GameState.Current.Players[0] = new Player
        {
            Name = "First Player",
            Tank = tanks[0],
            TankPrefab = TankPrefab,
        };
        GameState.Current.Players[1] = new Player
        {
            Name = "Guy 2",
            Tank = tanks[1],
            TankPrefab = TankPrefab,
        };
        GameState.Current.Players[2] = new Player
        {
            Name = "Third",
            Tank = tanks[2],
            TankPrefab = TankPrefab,
        };
        GameState.Current.Players[3] = new Player
        {
            Name = "Last Man",
            Tank = tanks[3],
            TankPrefab = TankPrefab,
        };
        SceneManager.LoadScene("Playground", LoadSceneMode.Single);
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
