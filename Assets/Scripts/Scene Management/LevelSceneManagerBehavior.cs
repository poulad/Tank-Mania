using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace TankMania
{
    public partial class LevelSceneManagerBehavior : MonoBehaviour
    {
        public CinemachineVirtualCamera VirtualCamera;

        public Text TimeoutText;

        public Text CurrentPlayerText;

        public Slider ChargeMeterSlider;

        public Image PauseMenuPanel;

        public KeyCode FireKey = KeyCode.Space;

#if UNITY_EDITOR
        public void Awake()
        {
            // Switch to Main Menu if game state is not initialized yet.
            // This allows running game from a Level scene.
            if (!GameManager.Current)
            {
                gameObject.SetActive(false);
                SceneManager.LoadScene(Constants.Scenes.MainMenu);
            }
        }
#endif

        public void Start()
        {
            GameManager.Current.InstantiateTanks(transform);
            foreach (var player in AllPlayers)
            {
                player.TankBehavior.Destroying += OnTankDestroying;
            }

            AllPlayers[0].Tank.transform.position = new Vector3(-4, 2, 0);
            AllPlayers[1].Tank.transform.position = new Vector3(0, 2, 0);
            AllPlayers[2].Tank.transform.position = new Vector3(3, 2, 0);
            AllPlayers[3].Tank.transform.position = new Vector3(6, 2, 0);

            AssignTurnToPlayer(AllPlayers[Random.Range(0, AllPlayers.Length)]);

            PauseMenuPanel.gameObject.SetActive(_isPaused);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Pause();

            if (_isPaused)
                return;

            UpdateTimer();
            CheckFireCharge();
            CheckForRandomDestroy();
        }

        public void Pause()
        {
            _isPaused = !_isPaused;

            foreach (var player in AllPlayers)
                player.TankBehavior.IsPaused = _isPaused;

            PauseMenuPanel.gameObject.SetActive(_isPaused);
        }

        public void Quit()
        {
            GameManager.Current.SwitchToScene(Constants.Scenes.MainMenu);
        }
    }
}
