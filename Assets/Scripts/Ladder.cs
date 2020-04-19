using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float m_feetOffset;
    public float HighestPoint { get; private set; }
    public float LowestPoint { get; private set; }

    private void Start()
    {
        var bounds = GetComponent<Collider2D>().bounds;
        HighestPoint = bounds.max.y;
        LowestPoint = bounds.min.y;
    }

    private void Reset()
    {
        var bounds = GetComponent<Collider2D>().bounds;
        HighestPoint = bounds.max.y;
        LowestPoint = bounds.min.y;
    }

    public bool IsTopEnd(Vector2 from)
    {
        return Mathf.Abs(from.y - HighestPoint) > Mathf.Abs(from.y - LowestPoint);
    }

    public Vector2 GetAttachingPosition(Vector2 from)
    {
        // only care about y axis here
        Vector2 retVal;
        retVal.x = transform.position.x;
        

        retVal.y = Mathf.Clamp(from.y, LowestPoint + m_feetOffset, HighestPoint);

        return retVal;
    }
}
