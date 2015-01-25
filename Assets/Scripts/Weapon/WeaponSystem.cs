using UnityEngine;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour
{

    public bool m_Rifle;
    public List<WeaponBase> m_ActiveWeapons;

    private WeaponBase m_SelectedWeapon;

    // Use this for initialization
    void Start()
    {
        m_ActiveWeapons = new List<WeaponBase>();
        if (m_Rifle)
        {
            m_ActiveWeapons.Add(new Rifle());
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
