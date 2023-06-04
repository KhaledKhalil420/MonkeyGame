using UnityEngine;

public class Vine : MonoBehaviour
{

    bool active;
    GameObject player;
    public LineRenderer render;
    public Transform renderStartPos;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        render.SetPosition(0, transform.position);
        render.SetPosition(1, renderStartPos.position);
    }
    
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        render.SetPosition(0, transform.position);
        if(!Input.GetButton("Jump"))
        {
            if(player != null)
            player.GetComponent<DistanceJoint2D>().enabled = false;
            //lerp vine to default position
            render.SetPosition(1, Vector2.Lerp(render.GetPosition(1), renderStartPos.position, 5 * Time.deltaTime));
            if(active)
            active = false;
        }
        if(!active) return;
        if(Input.GetButton("Jump"))
        {
            player.GetComponent<DistanceJoint2D>().enabled = true;
            render.SetPosition(1, player.transform.position);
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            active = true;
            player = other.gameObject;
        }
    }
}
