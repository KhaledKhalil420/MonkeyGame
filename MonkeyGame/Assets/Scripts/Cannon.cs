using Unity.Netcode;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float ShootingCoolDown;
    [SerializeField] private Transform ShootingPosition;

    [SerializeField] private GameObject CannonBall;

    private ParticleSystem ShootingParticle;

    void Start()
    {
        ShootingParticle = GetComponentInChildren<ParticleSystem>();
        Invoke(nameof(ShootBall), ShootingCoolDown);
    }

    void ShootBall()
    {
        //Repeat after ShootingCoolDown Time
        Invoke(nameof(ShootBall), ShootingCoolDown);
        
        //Spawn Bullet
        GameObject SpawnedObj = Instantiate(CannonBall, ShootingPosition.position, ShootingPosition.rotation);
        SpawnedObj.GetComponent<NetworkObject>().Spawn();
        

        //Effect
        ShootingParticle.Play();

        //Sounds~
        
    }
}
