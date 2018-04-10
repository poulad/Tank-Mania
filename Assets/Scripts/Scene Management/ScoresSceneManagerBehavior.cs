using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public class ScoresSceneManagerBehavior : MonoBehaviour
    {
        public GameObject[] Tanks;

        public Text[] TankNames;

        public Text[] Scores;

        public Button NextButton;

        public Camera Camera;

        public void Start()
        {
            Player[] players = GameManager.Current.Players
                .OrderByDescending(p => p.Score)
                .ToArray();

            for (int i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var nameText = TankNames[i];
                var scoreText = Scores[i];
                var tank = Tanks.Single(t =>
                    t.name == player.TankPrefab.GetComponent<TankBehavior>().name
                );

                float y = 3.5f - (2f * i);

                nameText.text = player.TankPrefab.GetComponent<TankBehavior>().Name;
                scoreText.text = player.Score + "";

                tank.transform.position = new Vector3(-6, y);
                nameText.GetComponent<Transform>().position = new Vector3(-4, y);
                scoreText.GetComponent<Transform>().position = new Vector3(4.5f, y);
            }

            NextButton.GetComponentInChildren<Text>().text = "To Level " + (GameManager.Current.CurrentLevel + 1);
        }

        public void ContinueToNextLevel()
        {
            GameManager.Current.SwitchToLevelScene(GameManager.Current.CurrentLevel + 1);
        }
    }
}
