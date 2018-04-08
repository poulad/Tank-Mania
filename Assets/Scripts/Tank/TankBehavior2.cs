using System;
using UnityEngine;
using UnityEngine.UI;

namespace TankMania
{
    public partial class TankBehavior
    {
        private GameObject _muzzle;

        private GameObject _launchPoint;

        private GameObject _weapon;

        private Rigidbody2D _rigidbody;

        private AudioSource _audioSource;

        private Animator _animator;

        private Slider _slider;

        private bool _isDriving;

        private bool _isPaused;

        private void OnExplosionFinished(object sender, EventArgs eventArgs)
        {
            if (Destroyed != null)
                Destroyed(this, EventArgs.Empty);
        }

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

                StartDriving();
            }
            else
            {
                StopDriving();
            }

            float moveV = Input.GetAxis("Vertical");
            if (0 < Mathf.Abs(moveV))
            {
                var rotationAxis = new Vector3(0, 0, Math.Sign(transform.localScale.x));
                _muzzle.transform.Rotate(rotationAxis, Mathf.Sign(moveV));

                float angle = _muzzle.transform.localEulerAngles.z < 180
                    ? _muzzle.transform.localEulerAngles.z
                    : _muzzle.transform.localEulerAngles.z - 360
                ;

                if (angle > MaxMuzzleAngle)
                    _muzzle.transform.localRotation = Quaternion.Euler(0, 0, MaxMuzzleAngle);
                else if (angle < MinMuzzleAngle)
                    _muzzle.transform.localRotation = Quaternion.Euler(0, 0, MinMuzzleAngle);
            }
        }

        private void Pause(bool pause)
        {
            _isPaused = pause;

            if (pause)
            {
                _audioSource.Pause();
                StopDriving();
            }
            else
            {
                _audioSource.UnPause();
            }
        }

        private void StartDriving()
        {
            _isDriving = true;
            _animator.SetBool("IsDriving", true);
        }

        private void StopDriving()
        {
            _isDriving = false;
            _animator.SetBool("IsDriving", false);
        }
    }
}
