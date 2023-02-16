using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPlayerPrefsTower : MonoBehaviour
{
    public Vector3 m_Position;
    public float speed = 1.0f;

    public void MoveToPosition()
    {
        StartCoroutine(MoveToPosition(transform, m_Position, speed));
    }
    private IEnumerator MoveToPosition(Transform transform, Vector3 position, float speed)
    {
        while (transform.position != position)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, position, speed * Time.deltaTime);
            yield return null;
        }
    }
}
