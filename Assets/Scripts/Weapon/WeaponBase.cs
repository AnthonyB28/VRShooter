using UnityEngine;
using System.Collections;

public abstract class WeaponBase
{
    public GameObject m_Weapon;

    protected GameObject m_Projectile;
    protected int m_AmmoReserveCurrent;
    protected int m_AmmoReserveMax;
    protected int m_AmmoClipMax;
    protected int m_AmmoClipCurrent;
    protected float m_RateOfFire;
    protected float m_RateOfFireCurrent = 0;
    protected float m_ReloadTime;
    protected float m_ReloadTimeCurrent = 0;
    protected bool m_IsSelected = false;
    protected bool m_IsReloading = false;
    protected bool m_IsFiring = false;

    abstract public void Update(); // To be called from WeaponSystem if selected
    abstract public void Fire();
    abstract public void Reload();
    abstract public void SpawnProjectile();

    public virtual void SetSelected(bool select)
    {
        m_IsSelected = select;
    }
}
