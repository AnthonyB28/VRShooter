using UnityEngine;
using System.Collections;

public abstract class WeaponBase
{
    public GameObject m_Weapon;
    public string m_Name;
    protected WeaponSystem m_WeaponSystem;
    protected GameObject m_Projectile;

    protected int m_AmmoReserveMax;
    private int _AmmoReserveCurrent;
    protected int m_AmmoReserveCurrent
    {
        get
        {
            return _AmmoReserveCurrent;
        }
        set
        {
            _AmmoReserveCurrent = value;
            if(m_WeaponSystem)
            {
                m_WeaponSystem.UpdateAmmoReserveCurr(value);
            }
        }
    }
    protected int m_AmmoClipMax;
    private int _AmmoClipCurrent;
    protected int m_AmmoClipCurrent
    {
        get
        {
            return _AmmoClipCurrent;
        }
        set
        {
            _AmmoClipCurrent = value;
            if (m_WeaponSystem)
            {
                m_WeaponSystem.UpdateAmmoClipCurr(value);
            }
        }
    }
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
