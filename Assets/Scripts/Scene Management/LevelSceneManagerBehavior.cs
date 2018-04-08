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

        public KeyCode FireKey = KeyCode.Space;

        public GameObject[] WeaponPrefabs;

#if UNITY_EDITOR
        public void Awake()
        {
            // Switch to Main Menu if game state is not initialized yet.
            // This allows running game from a Level scene.
            if (!GameManager.Current)
            {
                gameObject.SetActive(false);
                GameManager.ReturnToScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(Constants.Scenes.MainMenu);
            }
        }
#endif

        public void Start()
        {
            AssignComponents();

            GameManager.Current.InstantiateTanks(transform);
            foreach (var player in AllPlayers)
            {
                player.TankBehavior.Destroying += OnTankDestroying;
                player.TankBehavior.Destroyed += OnTankDestroyed;
            }

            AllPlayers[0].Tank.transform.position = new Vector3(-4, 2, 0);
            AllPlayers[1].Tank.transform.position = new Vector3(0, 2, 0);
            AllPlayers[2].Tank.transform.position = new Vector3(3, 2, 0);
            AllPlayers[3].Tank.transform.position = new Vector3(6, 2, 0);

            _pauseMenuPanel.gameObject.SetActive(false);
            AssignTurnToPlayer(AllPlayers[Random.Range(0, AllPlayers.Length)]);
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

            foreach (var player in AllPlayers)
                player.TankBehavior.IsPaused = _isPaused;

            _pauseMenuPanel.gameObject.SetActive(_isPaused);
        }

        public void Quit()
        {
            GameManager.Current.SwitchToScene(Constants.Scenes.MainMenu);
        }
    }
}
