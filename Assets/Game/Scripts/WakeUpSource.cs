using System.Collections.Generic;
using UnityEngine;

public class WakeUpSource : MonoBehaviour
{
    [field: SerializeField]
    public int Radius { get; private set; } = 2;

    [field: SerializeField]
    public Vector2Int LocalPosition { get; private set; } = Vector2Int.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.91f, 0.01f, 0.6f);

        foreach (var point in GetPoints())
        {
            Gizmos.DrawCube(point, new Vector3(1, 0.1f, 1));
        }
    }

    private IEnumerable<Vector3> GetPoints()
    {
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= Radius)
                {
                    yield return transform.position + new Vector3(x + LocalPosition.x, 0, y + LocalPosition.y);
                }
            }
        }
    }
}
