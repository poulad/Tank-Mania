using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TankMania
{
    [Obsolete]
    [RequireComponent(typeof(TilemapCollider2D))]
    public class TerrainBehavior : MonoBehaviour
    {
        private Tilemap _tilemap;

        public void Start()
        {
            _tilemap = GetComponent<Tilemap>();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Shell")
            {
                //var shellBehavior = other.gameObject.GetComponent<MissileBehavior>();

                var p = _tilemap.WorldToCell(other.gameObject.transform.position);
                DestructCellBlocks(new Vector2Int(p.x, p.y), 3 /*shellBehavior.BlockDestructionRadius*/);
            }
        }

        private void DestructCellBlocks(Vector2Int center, int radius)
        {
            for (int row = center.x - radius; row <= center.x + radius; row++)
                for (int col = center.y - radius; col <= center.y + radius; col++)
                    _tilemap.SetTile(new Vector3Int(row, col, 0), null);
        }
    }
}