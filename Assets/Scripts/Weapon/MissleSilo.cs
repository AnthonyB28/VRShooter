using UnityEngine;
using System.Collections.Generic;

public class MissleSilo : WeaponBase
{

    private Dictionary<int, GameObject> m_Targets;
    private bool m_Secondary;
    private bool m_IsTargeting;

    // Use this for initialization
    public MissleSilo(GameObject weapon, GameObject projectile, bool secondary, WeaponSystem sys)
    {
        m_Name = "Missles";
        m_WeaponSystem = sys;
        m_Targets = new Dictionary<int, GameObject>();
        m_Weapon = weapon;
        m_Projectile = projectile;
        m_AmmoClipMax = 6;
        m_AmmoClipCurrent = m_AmmoClipMax;
        m_AmmoReserveMax = 60;
        m_AmmoReserveCurrent = m_AmmoReserveMax;
        m_RateOfFire = 0f; // Not used here.
        m_ReloadTime = 6f;
        m_IsTargeting = false;
        m_Secondary = secondary;
    }

    public override void Update()
    {
        if (m_IsReloading)
        {
            m_ReloadTimeCurrent += Time.deltaTime;
            if (m_ReloadTimeCurrent >= m_ReloadTime)
            {
                m_IsReloading = false;
                m_ReloadTimeCurrent = 0;
            }
        }

        if (!m_IsReloading && !m_IsFiring && m_IsTargeting)
        {
            bool fired = m_Secondary ? Input.GetButtonUp("Fire2") : Input.GetButtonUp("Fire1");
            if (fired)
            {
                m_IsTargeting = false;
                m_IsFiring = true;
                if (m_Targets.Count > 0)
                {
                    foreach (GameObject target in m_Targets.Values)
                    {
                        target.renderer.material.color = Color.white;
                        --m_AmmoClipCurrent;
                        SpawnProjectile(target);
                    }
                    m_Targets.Clear();
                }
                m_IsFiring = false;
            }
        }
    }

    // For Silos, this is targeting. Firing missiles is done in Update.
    public override void Fire()
    {
        if (!m_IsFiring)
        {
            m_IsTargeting = true;
            if (m_Targets.Count < m_AmmoClipCurrent)
            {
                Ray ray = Camera.main.ScreenPointToRay(Camera.main.transform.GetComponent<Crosshair>().GetRandomSpread());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject obj = hit.collider.gameObject;
                    int id = hit.collider.gameObject.GetInstanceID();
                    // If we havent targeted the enemy already, add him to the collection.
                    if (obj.tag == "Enemy" && !m_Targets.ContainsKey(id))
                    {
                        obj.renderer.material.color = Color.red;
                        m_Targets.Add(id, obj);
                    }
                }
            }
            else if (m_AmmoClipCurrent == 0)
            {
                Reload();
            }
        }
    }

    private void SpawnProjectile(GameObject target)
    {
        if (target)
        {
            Transform spawn = m_Weapon.transform.GetChild(0);
            GameObject projectile = (GameObject)GameObject.Instantiate(m_Projectile, spawn.position, Camera.main.transform.rotation);
            projectile.GetComponent<Missile>().SetTarget(target);
        }
    }

    public override void SpawnProjectile()
    {
    }

    public override void Reload()
    {
        if (!m_IsReloading && m_AmmoClipCurrent != m_AmmoClipMax && m_AmmoReserveCurrent > 0)
        {
            m_IsReloading = true;
            int deficit = m_AmmoClipMax - m_AmmoClipCurrent;
            while (deficit > 0 && m_AmmoReserveCurrent > 0)
            {
                ++m_AmmoClipCurrent;
                --m_AmmoReserveCurrent;
                --deficit;
            }
        }
    }
}
