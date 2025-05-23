using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract IEnumerator RightClickDown(PlayerController player);
    public abstract IEnumerator RightClickUp(PlayerController player);
    public abstract IEnumerator LeftClickDown(PlayerController player);
    public abstract IEnumerator LeftClickUp(PlayerController player);
    public abstract IEnumerator WeaponUpdate(PlayerController player);
}
