using UnityEngine;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour
{

    public bool m_Rifle;
    public GameObject m_RifleObject;
    public GameObject m_RifleProjectile;

    public bool m_MachineGun;
    public GameObject m_MachineGunObject;
    public GameObject m_MachineGunProjectile;

    public bool m_MissileSilo;
    public GameObject m_MissileSiloObject;
    public GameObject m_MissileSiloProjectile;

    private List<WeaponBase> m_ActiveWeapons;
    private WeaponBase m_SelectedWeapon;

    // Use this for initialization
    void Start()
    {
        m_ActiveWeapons = new List<WeaponBase>();
        if (m_Rifle)
        {
            m_ActiveWeapons.Add(new Rifle(m_RifleObject, m_RifleProjectile));
        }

        if(m_MachineGun)
        {
            m_ActiveWeapons.Add(new MachineGun(m_MachineGunObject, m_MachineGunProjectile));
        }

        if(m_MissileSilo)
        {
            m_ActiveWeapons.Add(new MissleSilo(m_MissileSiloObject, m_MissileSiloProjectile));
        }

        if (m_ActiveWeapons.Count > 0)
        {
            SelectWeapon(m_ActiveWeapons[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_SelectedWeapon.Update();

        if (Input.GetButton("Fire1"))
        {
            m_SelectedWeapon.Fire();
        }
        else if(Input.GetButton("Reload"))
        {
            m_SelectedWeapon.Reload();
        }
    }

    void SelectWeapon(WeaponBase weapon)
    {
        if(m_SelectedWeapon != null)
        {
            m_SelectedWeapon.SetSelected(false);
        }
        m_SelectedWeapon = weapon;
        weapon.SetSelected(true);
    }
}
