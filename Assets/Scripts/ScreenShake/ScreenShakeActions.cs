using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private float _explosionIntensity = 3f;
    private float _swordHitIntensity = 1f;
    // Start is called before the first frame update
    void Start()
    {
        DefaultShotAction.OnShootHit += ShootHitActionOnAnyShootHit;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnOnAnyGrenadeExploded;
        MeleeAttackAction.OnAnyMeleeHit += MeleeActionOnAnyMeleeHit;
        KnockDownAction.OnAnyKnockDownHappened += MeleeActionOnAnyMeleeHit;
    }

    private void MeleeActionOnAnyMeleeHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(_swordHitIntensity);
    }

    private void GrenadeProjectile_OnOnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(_explosionIntensity);
    }

    private static void ShootHitActionOnAnyShootHit(object sender,  EventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
}
