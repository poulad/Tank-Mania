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
            RandomizePlayersOrder();
            GetComponentsOnScene();

            GameManager.Current.InstantiateTanks(transform);
            foreach (var player in _allPlayers)
            {
                player.TankBehavior.Destroying += OnTankDestroying;
                player.TankBehavior.Destroyed += OnTankDestroyed;
                player.Tank.name = "Tank - " + player.Name;
            }

            const float y = -.5f;
            _allPlayers[0].Tank.transform.position = new Vector3(-8, y, 0);
            _allPlayers[1].Tank.transform.position = new Vector3(-4, y, 0);
            _allPlayers[2].Tank.transform.position = new Vector3(3, y, 0);
            _allPlayers[3].Tank.transform.position = new Vector3(7, y, 0);

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
