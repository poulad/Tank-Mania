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

        public Text GameOverText;

        public Camera Camera;

        private bool _isGameOver;

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

            _isGameOver = GameManager.Current.CurrentLevel > 2;
            GameOverText.enabled = _isGameOver;
            NextButton.GetComponentInChildren<Text>().text = _isGameOver
                ? "To Main Menu"
                : "To Level " + (GameManager.Current.CurrentLevel + 1);
        }

        public void ContinueToNextLevel()
        {
            if (_isGameOver)
            {
                GameManager.Current.SwitchToScene(Constants.Scenes.MainMenu);
            }
            else
            {
                GameManager.Current.SwitchToLevelScene(GameManager.Current.CurrentLevel + 1);
            }
        }
    }
}
