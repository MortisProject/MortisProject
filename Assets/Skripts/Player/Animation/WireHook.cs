// Assets/Scripts/Player/Animation/WireHook.cs
using UnityEngine;

public class WireHook : MonoBehaviour
{
    public Transform target;
    public float speed = 50f;

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}