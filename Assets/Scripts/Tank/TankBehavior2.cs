using System;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class TankBehavior
    {
        private GameObject _muzzle;

        private GameObject _launchPoint;

        private Rigidbody2D _rigidbody;

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
            }

            float moveV = Input.GetAxis("Vertical");
            if (
                0 < Mathf.Abs(moveV)
            //-.2f <= _muzzle.transform.rotation.z &&
            //_muzzle.transform.rotation.z < .6f
            )
            {
                float zRotation = _muzzle.transform.rotation.z + (Mathf.Sign(moveV) * .6f);
                zRotation = Mathf.Clamp(zRotation, -15, 70);
                _muzzle.transform.rotation = Quaternion.Euler(0, 0, zRotation);

                //_muzzle.transform.Rotate(new Vector3(0, 0, zRotation));
                //if (_muzzle.transform.rotation.z < -.24f)
                {

                    //_muzzle.transform.rotation.Set(0, 0, -.2f, .1f);
                }
                Debug.LogWarning(_muzzle.transform.rotation.z);
                //Debug.Log(_muzzle.transform.rotation);
            }

            if (!_alreadyFired && Input.GetKey(FireKey))
            {
                Fire();
            }
        }

        private void Fire()
        {
            _alreadyFired = true;

            var shell = Instantiate(ShellPrefab, _launchPoint.transform.position, Quaternion.identity);
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
