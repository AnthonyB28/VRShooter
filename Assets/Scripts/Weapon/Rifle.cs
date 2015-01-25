using UnityEngine;
using System.Collections;

public class Rifle : WeaponBase
{

    public Rifle()
    {
        m_AmmoClipMax = 7;
        m_AmmoClipCurrent = m_AmmoClipMax;
        m_AmmoReserveMax = 21;
        m_AmmoReserveCurrent = m_AmmoReserveMax;
        m_RateOfFire = 0.5f;
        m_ReloadTime = 2.8f;
    }

    public override void Update()
    {
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
            }
            else
            {
                Reload();
            }
        }
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
