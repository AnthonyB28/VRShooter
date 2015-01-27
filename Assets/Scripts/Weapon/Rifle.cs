using UnityEngine;
using System.Collections;

public class Rifle : WeaponBase
{

    public Rifle(GameObject weapon, GameObject projectile)
    {
        m_Weapon = weapon;
        m_Projectile = projectile;
        m_AmmoClipMax = 7;
        m_AmmoClipCurrent = m_AmmoClipMax;
        m_AmmoReserveMax = 21;
        m_AmmoReserveCurrent = m_AmmoReserveMax;
        m_RateOfFire = 0.5f;
        m_ReloadTime = 2.8f;
    }

    public override void Update()
    {
        Transform spawn = m_Weapon.transform.GetChild(0);
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.transform.GetComponent<Crosshair>().GetRandomSpread());
        Debug.DrawRay(spawn.position, ray.direction, Color.red);
        if(m_IsFiring)
        {
            m_RateOfFireCurrent += Time.deltaTime;
            if(m_RateOfFireCurrent >= m_RateOfFire)
            {
                m_IsFiring = false;
                m_RateOfFireCurrent = 0;
            }
        }
        if(m_IsReloading)
        {
            m_ReloadTimeCurrent += Time.deltaTime;
            if(m_ReloadTimeCurrent >= m_ReloadTime)
            {
                m_IsReloading = false;
                m_ReloadTimeCurrent = 0;
            }
        }
    }

    public override void Fire()
    {
        if (!m_IsReloading && !m_IsFiring)
        {
            if (m_AmmoClipCurrent > 0)
            {
                m_IsFiring = true;
                --m_AmmoClipCurrent;
                SpawnProjectile();
            }
            else
            {
                Reload();
            }
        }
    }

    public override void SpawnProjectile()
    {
        Transform spawn = m_Weapon.transform.GetChild(0);

        Ray ray = Camera.main.ScreenPointToRay(Camera.main.transform.GetComponent<Crosshair>().GetRandomSpread());
        Debug.DrawRay(spawn.position, ray.direction, Color.red);
        RaycastHit hit;
        Vector3 lookPoint;
        float distance = 40f;
        if (Physics.Raycast(ray,out hit))
        {
            lookPoint = hit.point;
        }
        else
        {
            lookPoint = ray.GetPoint(distance);
        }
        GameObject projectile = (GameObject) GameObject.Instantiate(m_Projectile, spawn.position, Camera.main.transform.rotation);
        projectile.transform.LookAt(lookPoint);
    }

    public override void Reload()
    {
        if(!m_IsReloading && m_AmmoClipCurrent != m_AmmoClipMax && m_AmmoReserveCurrent > 0)
        {
            m_IsReloading = true;
            int deficit = m_AmmoClipMax - m_AmmoClipCurrent;
            while(deficit > 0 && m_AmmoReserveCurrent > 0)
            {
                ++m_AmmoClipCurrent;
                --m_AmmoReserveCurrent;
                --deficit;
            }
        }
    }
}
