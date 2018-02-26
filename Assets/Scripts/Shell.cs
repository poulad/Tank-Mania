using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// ReSharper disable once CheckNamespace
public class Shell : MonoBehaviour
{
    private AudioSource _audioSource;

    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        Destroy(gameObject, 3);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        {
            var tileMap = other.GetComponent<Tilemap>();
            if (tileMap == null) return;


        }


        return;
        {
            var tileMap = other.GetComponent<Tilemap>();
            if (tileMap == null) return;
            var bounds = new BoundsInt(-2, -2, -1, 4, 4, 4);
            var tiles = tileMap.GetTilesBlock(bounds);
            tiles = tiles.Where(_ => _ != null).ToArray();
            foreach (var t in tiles)
            {
                var tt = (Tile)t;
                //tileMap.SetTile(tt.gameObject);
                try
                {
                    //DestroyImmediate(tt, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return;
        }

        Destroy(other.gameObject, 2);
        _audioSource.Play();
    }
}
