using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Abilites : EasyNetworkBehaviour
{
    public enum PlayerUpgrade {SpeedBoost, DoubleJump, Slowness, CameraFlip}

    [SerializeField] private PlayerUpgrade PowerUp;
    private GameObject Player;

    private BoxCollider2D BoxCollider;
    private SpriteRenderer spriteRenderer;

    private float OldPlayerSpeed;

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
            StopAllCoroutines();

            switch(PowerUp)
            {
                case PlayerUpgrade.SpeedBoost:
                OldPlayerSpeed = Player.GetComponent<PlayerMovement>().Speed;
                StartCoroutine(SpeedBoost());
                break;


                case PlayerUpgrade.Slowness:
                OldPlayerSpeed = Player.GetComponent<PlayerMovement>().Speed;
                StartCoroutine(Slowness());
                break;

                case PlayerUpgrade.DoubleJump:
                Jump();
                break;

                case PlayerUpgrade.CameraFlip:
                StartCoroutine(CameraFlip());
                break;
            }
        }
    }

    IEnumerator SpeedBoost()
    {
        OldPlayerSpeed = Player.GetComponent<PlayerMovement>().Speed;
        Player.GetComponent<PlayerMovement>().Speed = Player.GetComponent<PlayerMovement>().Speed * 1.5f;

        BoxCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        Player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
        NetworkObject.Despawn(true);
    }

    IEnumerator Slowness()
    {
        Player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
        Player.GetComponent<PlayerMovement>().Speed = Player.GetComponent<PlayerMovement>().Speed / 1.5f;

        BoxCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        Player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
        NetworkObject.Despawn(true);
    }

    public void Jump()
    {
        Player.GetComponent<PlayerMovement>().Jump(600);
        NetworkObject.Despawn(true);
    }

    IEnumerator CameraFlip()
    {
        Camera.main.transform.eulerAngles = new Vector3(0, 0, 180);

        BoxCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0);
        NetworkObject.Despawn(true);
    }
}
