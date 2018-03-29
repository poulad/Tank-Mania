using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class LevelSceneManagerBehavior : MonoBehaviour
    {
        public GameObject VirtualCameraObject;

        public Text TimeoutText;

        public Text CurrentPlayerText;

        public Slider ChargeMeterSlider;

        public KeyCode FireKey = KeyCode.Space;

        public void Start()
        {
            _virtualCamera = VirtualCameraObject.GetComponent<CinemachineVirtualCamera>();

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
        }

        public void Update()
        {
            UpdateTimer();
            WatchFireCharge();
            CheckForRandomDestroy();
        }
    }
}
