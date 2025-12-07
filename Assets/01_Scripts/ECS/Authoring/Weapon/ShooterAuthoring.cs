using UnityEngine;

public class ShooterAuthoring : MonoBehaviour
{
    public Faction OwnerFaction;

    public GameObject ProjectilePrefab;

    public Vector3 MuzzleOffset;

    public int Count = 1;

    public float MuzzleDistance;
    public float Damage = 10f;
    public float FireRate = 1f;
    public float Speed = 1f;
    public float Duration = 1f;

    public bool StartCanFire;
}