using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Vector2 CheckPosition;

    [SerializeField] private SpriteRenderer SpriteRenderer;

    private void Start()
    {
        CheckPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Damage"))
        {
            //Go to lastCheckpoint
            GotoCheckpoint();
            StartCoroutine(DamageEffect());
        }
    }

    IEnumerator DamageEffect()
    {
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        SpriteRenderer.color = Color.white;
    }

    void GotoCheckpoint()
    {
        transform.position = CheckPosition;
    }
}
