using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    [Header("Follow"), Space(10)]
    public float FollowSmoothness;
    public Vector3 Offset;
    private GameObject[] Players;
    private Transform Target;
    private Animator Anim;

    [Header("CameraShake"), Space(10)]
    public float ShakeStrength;
    public float ShakeTime;
    private Transform Cam;

    private void Start()
    {
        Cam = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if(Instance == null) Instance = this;

        if(Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(ShakeCamera(ShakeStrength, ShakeTime));
        }

        if(Target != null) 
        return;

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject P in Players)
        {
            if(P.GetComponent<PlayerMovement>().IsOwner)
            Target = P.transform;
        }

    }



    private void FixedUpdate()
    {
        if(Target != null)
        transform.position = Vector3.Lerp(transform.position, Target.position + Offset, FollowSmoothness * Time.fixedDeltaTime);
    }
        
    public IEnumerator ShakeCamera(float Strength, float Duration)
    {
        Debug.Log("s");
        float ElapsedTime = 0;
        Vector3 OriginalPos = Cam.localPosition;

        while(ElapsedTime < Duration)
        {

            Cam.localPosition = new Vector3(Random.Range(-1, 1) * Strength, Random.Range(-1, 1) * Strength, -11);
            ElapsedTime += Time.deltaTime;
            yield return null;
        }

        Cam.localPosition = OriginalPos;
    }
}
