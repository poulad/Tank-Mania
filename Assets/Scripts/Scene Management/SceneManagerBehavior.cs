using Cinemachine;
using UnityEngine;

namespace TankMania
{
    public class SceneManagerBehavior : MonoBehaviour
    {
        public GameObject[] Tanks;

        public GameObject VirtualCameraObject;

        private CinemachineVirtualCamera _virtualCamera;

        public void Start()
        {
            _virtualCamera = VirtualCameraObject.GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = Tanks[0].transform;
        }
    }
}
