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
            Player1NameText.text = GameManager.Current.Players[0].Name;
            Player2NameText.text = GameManager.Current.Players[1].Name;
            Player3NameText.text = GameManager.Current.Players[2].Name;
        }

        public void ContinueToNextLevel()
        {
            GameManager.Current.SwitchToScene(Constants.Scenes.Playground);
        }
    }
}
