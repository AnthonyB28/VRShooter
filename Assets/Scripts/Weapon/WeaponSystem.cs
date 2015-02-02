using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour
{
    public bool m_Secondary;
    public Text m_WeaponTitle;
    public Text m_AmmoCurrent;
    public Text m_AmmoReserve;
    public Image m_ReloadCircle;
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
    private int m_SelectedWeaponIndex = 0;

    // Use this for initialization
    void Start()
    {
        m_ActiveWeapons = new List<WeaponBase>();
        if (m_Rifle)
        {
            m_ActiveWeapons.Add(new Rifle(m_RifleObject, m_RifleProjectile, this));
        }

        if(m_MachineGun)
        {
            m_ActiveWeapons.Add(new MachineGun(m_MachineGunObject, m_MachineGunProjectile, this));
        }

        if(m_MissileSilo)
        {
            m_ActiveWeapons.Add(new MissleSilo(m_MissileSiloObject, m_MissileSiloProjectile, m_Secondary, this));
        }

        if (m_ActiveWeapons.Count > 0)
        {
            SelectWeapon(m_ActiveWeapons[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (!m_Secondary)
        {
            if (scroll > 0)
            {
                ++m_SelectedWeaponIndex;
                if(m_SelectedWeaponIndex == m_ActiveWeapons.Count)
                {
                    m_SelectedWeaponIndex = 0;
                }
                SelectWeapon(m_ActiveWeapons[m_SelectedWeaponIndex]);
            }
            else if (scroll < 0)
            {
                --m_SelectedWeaponIndex;
                if (m_SelectedWeaponIndex <= 0)
                {
                    m_SelectedWeaponIndex = m_ActiveWeapons.Count-1;
                }
                SelectWeapon(m_ActiveWeapons[m_SelectedWeaponIndex]);
            }
        }
        bool fired = m_Secondary ? Input.GetButton("Fire2") : Input.GetButton("Fire1");

        m_SelectedWeapon.Update();

        if (fired)
        {
            m_SelectedWeapon.Fire();
        }
        else if(Input.GetButton("Reload"))
        {
            m_SelectedWeapon.Reload();
        }
    }

    public void UpdateAmmoClipCurr(int ammo)
    {
        if(m_AmmoCurrent)
        {
            m_AmmoCurrent.text = ammo.ToString();
        }
    }

    public void UpdateAmmoReserveCurr(int ammo)
    {
        if (m_AmmoReserve)
        {
            m_AmmoReserve.text = ammo.ToString();
        }
    }

    void SelectWeapon(WeaponBase weapon)
    {
        if(m_SelectedWeapon != null)
        {
            m_SelectedWeapon.SetSelected(false);
        }
        m_SelectedWeapon = weapon;
        if(m_WeaponTitle)
        {
            m_WeaponTitle.text = weapon.m_Name;
        }
        UpdateReloadingTimeCurr(1f, 1f);
        weapon.SetSelected(true);
    }

    public void UpdateReloadingTimeCurr(float time, float max)
    {
        if(m_ReloadCircle)
        {
            m_ReloadCircle.GetComponent<ReloadShader>().ChangeCircle(1-(time / max));
        }
    }

}
