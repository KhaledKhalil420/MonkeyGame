using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu]
public class weaponInfo : ScriptableObject
{
    public NetworkObject bullet;
    public Sprite weaponSprite;
    public float timeBtwShots;
    public float damage;
    public float bulletSpeed;
}
