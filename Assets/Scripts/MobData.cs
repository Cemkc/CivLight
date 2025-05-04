using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Mob", fileName = "Mob")]
public class MobData : ScriptableObject
{
    public MobType MobType;
    public int Health;
    public int Damage;
}
