using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseCamera : MonoBehaviour
{
    public float smoothTime;
    private Vector3 velocity;
    public Vector3 offset;

    public Vector3 minValue, maxValue;

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         Vector3 bounds = new Vector3(
            Mathf.Clamp(mousePos.x, minValue.x, maxValue.x),
            Mathf.Clamp(mousePos.y, minValue.y, maxValue.y),
            Mathf.Clamp(-10, -10, maxValue.z)
            );

        transform.position = Vector3.SmoothDamp(transform.position, bounds + offset, ref velocity, smoothTime);
    }
}
