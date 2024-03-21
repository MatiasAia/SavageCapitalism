using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileSnapping : MonoBehaviour
{
    private void Awake()
    {
        if(Application.isPlaying)
            enabled = false;
    }

    private void Update()
    {
        Snap(transform);
    }

    public static void Snap(Transform transform)
    {
        transform.position = Snap(transform.position);
    }

    public static Vector3 Snap(Vector3 vector3)
    {
        return new Vector3(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z));
    }
}
