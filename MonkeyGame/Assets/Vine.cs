using UnityEngine.U2D;
using UnityEngine;

public class Vine : MonoBehaviour
{
    private SpriteShapeController shape;
    private Transform Player;
    private Vector3 StartingPos;

    private void Start()
    {
        shape = GetComponent<SpriteShapeController>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        StartingPos = shape.spline.GetPosition(0);
    }

    private void Update()
    {
        if(PlayerVine.IsUsingVine)
        shape.spline.SetPosition(0, Player.position + new Vector3(1.4f, 0.8f, 0));

        else
        shape.spline.SetPosition(0, StartingPos);
    }
}
