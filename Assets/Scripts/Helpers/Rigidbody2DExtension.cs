using System;
using UnityEngine;

namespace TankMania.Helpers
{
    public static class Rigidbody2DExtension
    {
        /// <remarks>https://stackoverflow.com/questions/34250868/unity-addexplosionforce-to-2d</remarks>
        public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
        {
            var explosionDir = rb.position - explosionPosition;
            var explosionDistance = explosionDir.magnitude;

            // Normalize without computing magnitude again
            if (Math.Abs(upwardsModifier) < 0.00001)
                explosionDir /= explosionDistance;
            else
            {
                // From Rigidbody.AddExplosionForce doc:
                // If you pass a non-zero value for the upwardsModifier parameter, the direction
                // will be modified by subtracting that value from the Y component of the centre point.
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }

            rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
        }
    }
}
