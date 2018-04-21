using Cinemachine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace TankMania
{
    public partial class LevelSceneManagerBehavior : MonoBehaviour
    {
        public CinemachineVirtualCamera VirtualCamera;

        public Canvas ScreenCanvas;

        public float TurnTimeout;

        public GameObject Lava;

        public Vector3[] InitialPositions;

        public GameObject[] WeaponPrefabs;

#if UNITY_EDITOR
        public void Awake()
        {
            // Switch to Main Menu if game state is not initialized yet.
            // This allows running game directly from a Level scene using Unity Editor.
            if (!GameManager.Current)
            {
                GameManager.ReturnToScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(Constants.Scenes.MainMenu);
                gameObject.SetActive(false);
            }
        }
#endif

        public void Start()
        {
            GetComponentsOnScene();
            RandomizePlayersOrder();
            InitializeTanks();

            _pauseMenuPanel.gameObject.SetActive(false);
            AssignTurnToPlayer(Random.Range(0, _allPlayers.Length));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                Pause();

            if (_isPaused)
                return;

            if (_isWaitingForPlayerMove)
            {
                CheckForPlayerMove();
            }
            else
            {
                UpdateTimer();
                CheckFireCharge();
                CheckForRandomDestroy();
            }
        }

        public void Pause()
        {
            _isPaused = !_isPaused;

            foreach (var player in ActivePlayers)
                player.TankBehavior.IsPaused = _isPaused;

            _pauseMenuPanel.gameObject.SetActive(_isPaused);
        }

        public void Quit()
        {
            GameManager.Current.SwitchToScene(Constants.Scenes.MainMenu);
        }
    }
}
