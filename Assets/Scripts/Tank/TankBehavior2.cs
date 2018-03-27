using System;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class TankBehavior
    {
        private const float MaxMuzzleAngle = 50;

        private const float MinMuzzleAngle = -12;

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
            if (0 < Mathf.Abs(moveV))
            {
                var rotationAxis = new Vector3(0, 0, Math.Sign(transform.localScale.x));
                _muzzle.transform.Rotate(rotationAxis, Mathf.Sign(moveV));

                //float angle = Math.Sign(transform.rotation.z * _muzzle.transform.localRotation.z) *
                //    Quaternion.Angle(transform.rotation, _muzzle.transform.rotation);

                //Debug.Log(angle);

                //if (angle > MaxMuzzleAngle)
                //    _muzzle.transform.localRotation = Quaternion.AngleAxis(MaxMuzzleAngle, new Vector3(0, 0, 1));
                //else if (angle < MinMuzzleAngle)
                //    _muzzle.transform.localRotation = Quaternion.AngleAxis(MinMuzzleAngle, new Vector3(0, 0, 1));
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
