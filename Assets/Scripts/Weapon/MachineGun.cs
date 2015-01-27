using UnityEngine;
using System.Collections;

public class MachineGun : Rifle {

    public MachineGun(GameObject weapon, GameObject projectile)
        : base(weapon, projectile)
    {
        m_AmmoClipMax = 30;
        m_AmmoClipCurrent = m_AmmoClipMax;
        m_AmmoReserveMax = 90;
        m_AmmoReserveCurrent = m_AmmoReserveMax;
        m_RateOfFire = 0.1f;
        m_ReloadTime = 2.5f;
    }
}
