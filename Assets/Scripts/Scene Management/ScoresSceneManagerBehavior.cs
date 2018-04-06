using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public class ScoresSceneManagerBehavior : MonoBehaviour
    {
        public Text Player1NameText;

        public Text Player1ScoreText;

        public Text Player2NameText;

        public Text Player2ScoreText;

        public Text Player3NameText;

        public Text Player3ScoreText;

        public void Start()
        {
            Player[] highScorePlayers = GameManager.Current.Players
                .OrderByDescending(p => p.Score)
                .Take(3)
                .ToArray();

            Player1NameText.text = highScorePlayers[0].Name;
            Player1ScoreText.text = highScorePlayers[0].Score + "";

            Player2NameText.text = highScorePlayers[1].Name;
            Player2ScoreText.text = highScorePlayers[1].Score + "";

            Player3NameText.text = highScorePlayers[2].Name;
            Player3ScoreText.text = highScorePlayers[2].Score + "";
        }

        public void ContinueToNextLevel()
        {
            GameManager.Current.SwitchToScene(Constants.Scenes.Playground);
        }
    }
}
