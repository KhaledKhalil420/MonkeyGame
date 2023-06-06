using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class weapon : NetworkBehaviour
{
    public weaponInfo info;

    public SpriteRenderer GFX;

    public Transform shotPoint;

    float timeBtwShoots;

    Vector2 defaultScale;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        defaultScale = transform.localScale;
        GFX.sprite = info.weaponSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsClient) return;

        faceMouse();

        shoot();
    }

    void faceMouse()
    {
        Vector3 diffrance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(diffrance.y, diffrance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, rotZ);

        if (rotZ < 89 && rotZ > -89)
        {
            //transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
        }
        else
        {
            //transform.eulerAngles = new Vector2(transform.eulerAngles.x, -180);
        }
    }

    void shoot()
    {
        if(timeBtwShoots <= 0)
        {
            if(Input.GetMouseButtonDown(0))
            {
                //spawn bullet ass network object
                NetworkObject bullet = Instantiate(info.bullet, shotPoint.position, shotPoint.rotation);
                lobbyManager.Instance.spawnNetWorkObject(bullet);
                bullet.GetComponent<Rigidbody2D>().AddForce(transform.right * info.bulletSpeed);
                timeBtwShoots = info.timeBtwShots;
            }
        }
        else
        {
            timeBtwShoots -= Time.deltaTime;
        }
    }
}
