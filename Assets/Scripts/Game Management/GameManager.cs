﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankMania
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Current;

        public int CurrentLevel { get; private set; }

        public Player[] Players { get; set; }

#if UNITY_EDITOR
        public static string ReturnToScene;
#endif

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Current = this;
            Current.Players = new Player[4];
        }

        public void InstantiateTanks(Transform parent)
        {
            foreach (var player in Players)
            {
                player.Tank = Instantiate(player.TankPrefab, parent);
            }

            foreach (var sr in Players[1].Tank.GetComponentsInChildren<SpriteRenderer>())
                sr.color = Color.red;

            foreach (var sr in Players[2].Tank.GetComponentsInChildren<SpriteRenderer>())
                sr.color = Color.blue;

            foreach (var sr in Players[3].Tank.GetComponentsInChildren<SpriteRenderer>())
                sr.color = Color.gray;
        }

        public void SwitchToScene(string scene)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }

        public void SwitchToLevelScene(int level)
        {
            CurrentLevel = 1;
            SwitchToScene("Level " + CurrentLevel);
        }
    }
}