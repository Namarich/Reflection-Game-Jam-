using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Explosion
{
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    {
        var explosionDir = rb.position - explosionPosition;
        //var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        
        if (upwardsModifier != 0)
        {
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        float explosionDistance = 2f;
        rb.AddForce(Mathf.Lerp(0, explosionForce, (explosionDistance)) * explosionDir, mode);
    }

}
