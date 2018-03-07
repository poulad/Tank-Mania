using System;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class TankBehavior
    {
        private GameObject _muzzle;

        private Rigidbody2D _rigidbody;

        private GameObject _healthBar;

        private AudioSource _audioSource;

        private bool _alreadyFired;

        private Slider _slider;

        private void ControlMovement()
        {
            float moveH = Input.GetAxis("Horizontal");
            if (0 < Mathf.Abs(moveH))
            {
                _rigidbody.velocity = new Vector2(moveH, _rigidbody.velocity.y);

                transform.localScale = new Vector3(
                    transform.localScale.x * Mathf.Sign(moveH * transform.localScale.x),
                    transform.localScale.y
                );

                _healthBar.transform.localScale = new Vector3(
                    _healthBar.transform.localScale.x * Mathf.Sign(moveH * _healthBar.transform.localScale.x),
                    _healthBar.transform.localScale.y
                );
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
            shell.transform.localScale = new Vector3(
                shell.transform.localScale.x * Mathf.Sign(transform.localScale.x),
                shell.transform.localScale.y
            );
            shell.velocity = 8 * (transform.localScale.x > 0 ? Vector3.right : Vector3.left);

            if (Fired != null)
                Fired(this, EventArgs.Empty);
        }
    }
}
