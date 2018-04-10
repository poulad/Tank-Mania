using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankMania
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Current;

        public int CurrentLevel { get; private set; }

        public Player[] Players { get; set; }

#if UNITY_EDITOR
        public static string ReturnToScene;
#endif

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Current = this;
            Current.Players = new Player[4];
        }

        public void InstantiateTanks(Transform parent)
        {
            foreach (var player in Players)
            {
                player.Name = player.TankPrefab.GetComponent<TankBehavior>().Name;
                player.Tank = Instantiate(player.TankPrefab, parent);
            }
        }

        public void SwitchToScene(string scene)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }

        public void SwitchToLevelScene(int level)
        {
            CurrentLevel = level;
            SwitchToScene("Level " + CurrentLevel);
        }
    }
}
