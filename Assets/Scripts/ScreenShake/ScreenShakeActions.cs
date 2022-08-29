using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private float _explosionIntensity = 5f;
    private float _swordHitIntensity = 2f;
    // Start is called before the first frame update
    void Start()
    {
        DefaultShotAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnOnAnyGrenadeExploded;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(_swordHitIntensity);
    }

    private void GrenadeProjectile_OnOnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(_explosionIntensity);
    }

    private static void ShootAction_OnAnyShoot(object sender, OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
}
