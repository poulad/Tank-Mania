using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankMania
{
    public class TankBehavior : MonoBehaviour
    {
        public Rigidbody2D Shell;

        public KeyCode FireKey = KeyCode.Space;

        [HideInInspector]
        public bool HasCurrentTurn { get; private set; }

        public event EventHandler<EventArgs> Fired;

        private Rigidbody2D _rigidbody;

        private SpriteRenderer _spriteRenderer;

        private AudioSource _audioSource;

        private bool _alreadyFired;

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
        }

        public void FixedUpdate()
        {
            _audioSource.pitch = Random.Range(.9f, 1.1f);
            if (HasCurrentTurn)
            {
                ControlMovement();
            }
        }

        public void TakeCurrentTurn()
        {
            HasCurrentTurn = true;
            _alreadyFired = false;
        }

        public void StopTurn()
        {
            HasCurrentTurn = false;
        }

        private void ControlMovement()
        {
            float moveH = Input.GetAxis("Horizontal");
            if (0 < Mathf.Abs(moveH))
            {
                _rigidbody.velocity = new Vector2(moveH, _rigidbody.velocity.y);
                bool spriteIsInRightDirection = (moveH > 0 && !_spriteRenderer.flipX) ||
                                                (moveH < 0 && _spriteRenderer.flipX);
                if (!spriteIsInRightDirection)
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
            }

            if (!_alreadyFired && Input.GetKey(FireKey))
            {
                _alreadyFired = true;
                if (Fired != null) Fired(this, EventArgs.Empty);

                var shellInstance = Instantiate(Shell, transform.position, Quaternion.identity);
                shellInstance.velocity = 2.5f * (_spriteRenderer.flipX ? Vector3.left : Vector3.right);
            }
        }
    }
}
