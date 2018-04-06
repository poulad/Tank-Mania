using UnityEngine;

namespace TankMania
{
    public class PlaygroundSceneManagerBehavior : LevelSceneManagerBehavior
    {
        public GameObject TankPrefab;

#if UNITY_EDITOR
        public new void Awake()
        {
            //var player1 = new Player("Player 1")
            //{
            //    TankPrefab = TankPrefab
            //};
            //var player2 = new Player("Player 2")
            //{
            //    TankPrefab = TankPrefab
            //};

            //GameManager.Current.Players = new[] { player1, player2 };
            GameManager.ReturnToScene = Constants.Scenes.Playground;

            base.Awake();
        }
#endif

        public new void Start()
        {
            foreach (var player in AllPlayers)
            {
                player.Tank = Instantiate(player.TankPrefab, transform);
                player.Tank.name = "Tank - " + player.Name;
            }

            AllPlayers[0].Tank.transform.position = new Vector3(-2, 2, 0);
            AllPlayers[1].Tank.transform.position = new Vector3(2, 2, 0);

            AssignTurnToPlayer(AllPlayers[0]);

            PauseMenuPanel.gameObject.SetActive(false);
        }
    }
}
