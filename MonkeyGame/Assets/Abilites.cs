using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Abilites : EasyNetworkBehaviour
{
    public enum PlayerUpgrade {SpeedBoost, DoubleJump, Slowness, CameraFlip, RandomUpgrade}

    [SerializeField] private PlayerUpgrade PowerUp;
    private GameObject Player;

    private BoxCollider2D BoxCollider;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        BoxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Player = other.gameObject;

            switch(PowerUp)
            {
                case PlayerUpgrade.SpeedBoost:
                StartCoroutine(SpeedBoost());
                break;

                case PlayerUpgrade.DoubleJump:
                
                break;

                case PlayerUpgrade.Slowness:
                StartCoroutine(Slowness());
                break;

                case PlayerUpgrade.CameraFlip:
                StartCoroutine(CameraFlip());
                break;

                case PlayerUpgrade.RandomUpgrade:
                
                break;
            }
        }
    }

    IEnumerator SpeedBoost()
    {
        float OldPlayerSpeed = Player.GetComponent<PlayerMovement>().Speed;
        Player.GetComponent<PlayerMovement>().Speed = Player.GetComponent<PlayerMovement>().Speed * 1.5f;

        BoxCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        Player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
    }

    IEnumerator Slowness()
    {
        float OldPlayerSpeed = Player.GetComponent<PlayerMovement>().Speed;
        Player.GetComponent<PlayerMovement>().Speed = Player.GetComponent<PlayerMovement>().Speed / 1.5f;

        BoxCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        Player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
    }

    IEnumerator CameraFlip()
    {
        Camera.main.transform.eulerAngles = new Vector3(0, 0, 180);

        BoxCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void DespawnPlayer()
    {
        GetComponent<NetworkObject>().Despawn(false);
    }
}
