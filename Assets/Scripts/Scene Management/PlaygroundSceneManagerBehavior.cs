using System.Linq;
using UnityEngine;

namespace TankMania
{
    public class PlaygroundSceneManagerBehavior : LevelSceneManagerBehavior
    {
        public new void Start()
        {
            TurnTimeout = 30;
            GameManager.Current.Players = GameManager.Current.Players.Take(2).ToArray();

            foreach (var player in AllPlayers)
            {
                player.Tank = Instantiate(player.TankPrefab, transform);
                player.Tank.name = "Tank - " + player.Name;
            }

            AllPlayers[0].Tank.transform.position = new Vector3(-3, 2, 0);
            AllPlayers[1].Tank.transform.position = new Vector3(3, 2, 0);

            AssignTurnToPlayer(AllPlayers[0]);

            PauseMenuPanel.gameObject.SetActive(false);
        }
    }
}
