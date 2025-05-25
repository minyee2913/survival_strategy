using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponListData
{
    public Weapon weapon;
    public float per;
}

[CreateAssetMenu(fileName = "WeaponSet", menuName = "weapons/WeaponSet", order = int.MaxValue)]
public class WeaponSet : ScriptableObject
{
    public List<WeaponListData> weaponList;

    public Weapon GetRandomWeapon()
    {
        List<WeaponListData> weapons = ShuffleAndCopy(weaponList);

        foreach (WeaponListData weapon in weapons)
        {
            if (Random.Range(0f, 100f) <= weapon.per)
            {
                return weapon.weapon;
            }
        }

        return null;
    }

    public static List<T> ShuffleAndCopy<T>(List<T> original)
    {
        List<T> copy = new List<T>(original); // 복사
        for (int i = copy.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            T temp = copy[i];
            copy[i] = copy[rand];
            copy[rand] = temp;
        }
        return copy;
    }

}
