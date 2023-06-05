using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Abilites : EasyNetworkBehaviour
{
    public enum PlayerUpgrade {SpeedBoost, DoubleJump, Slowness, CameraFlip, RandomUpgrade}

    [SerializeField] private PlayerUpgrade PowerUp;

    private GameObject[] Players;

    float OldPlayerSpeed;

    private void Start()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        OldPlayerSpeed = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().Speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
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

            DestroyNObject(gameObject);
        }
    }

    IEnumerator SpeedBoost()
    {
        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerMovement>().Speed = player.GetComponent<PlayerMovement>().Speed * 3;
        }

        yield return new WaitForSeconds(5);

        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
        }
    }

    IEnumerator Slowness()
    {
        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerMovement>().Speed = player.GetComponent<PlayerMovement>().Speed / 2;
        }

        yield return new WaitForSeconds(5);

        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerMovement>().Speed = OldPlayerSpeed;
        }
    }

    IEnumerator CameraFlip()
    {
        Camera.current.transform.eulerAngles = new Vector3(0, 0, 180);

        yield return new WaitForSeconds(5);

        Camera.current.transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
