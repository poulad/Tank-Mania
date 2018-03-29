using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class SceneManagerBehavior : MonoBehaviour
    {
        public GameObject VirtualCameraObject;

        public Text TimeoutText;

        public Text CurrentPlayerText;

        public Slider ChargeMeterSlider;

        public KeyCode FireKey = KeyCode.Space;

        public void Start()
        {
            _virtualCamera = VirtualCameraObject.GetComponent<CinemachineVirtualCamera>();

            foreach (var player in AllPlayers)
            {
                player.TankBehavior.Destroying += OnTankDestroying;
            }

            AssignTurnToPlayer(AllPlayers.First());
        }

        public void Update()
        {
            UpdateTimer();
            WatchFireCharge();
            CheckForRandomDestroy();
        }
    }
}
