using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankMania
{
    public class TankBehavior : MonoBehaviour
    {
        public Rigidbody2D ShellPrefab;

        public KeyCode FireKey = KeyCode.Space;

        [HideInInspector]
        public bool HasCurrentTurn { get; private set; }

        public event EventHandler<EventArgs> Fired;

        private GameObject _muzzle;

        private Rigidbody2D _rigidbody;

        private SpriteRenderer _spriteRenderer;

        private AudioSource _audioSource;

        private bool _alreadyFired;

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();

            _muzzle = GetComponentsInChildren<Transform>()
                .Single(t => t.gameObject.name == "Muzzle")
                .gameObject;
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
                bool isSpriteInCorrectDirection = (moveH > 0 && !_spriteRenderer.flipX) || (moveH < 0 && _spriteRenderer.flipX);

                if (!isSpriteInCorrectDirection)
                {
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                    _muzzle.transform.localPosition = new Vector3(-
                        _muzzle.transform.localPosition.x,
                        _muzzle.transform.localPosition.y
                    );
                }
            }

            if (!_alreadyFired && Input.GetKey(FireKey))
            {
                Fire();
            }
        }

        private void Fire()
        {
            _alreadyFired = true;

            var shell = Instantiate(ShellPrefab, _muzzle.transform.position, Quaternion.identity);
            var shellSpriteRenderer = shell.GetComponent<SpriteRenderer>();
            shellSpriteRenderer.flipX = _spriteRenderer.flipX;
            shell.velocity = 5 * (_spriteRenderer.flipX ? Vector3.left : Vector3.right);

            if (Fired != null)
                Fired(this, EventArgs.Empty);
        }
    }
}
