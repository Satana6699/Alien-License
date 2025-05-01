using UnityEngine;

namespace Game.Scripts
{
    public class PuzzleBlock : MonoBehaviour
    {
        public Vector2Int Size = Vector2Int.one;
        public Vector2Int CurrentPos = Vector2Int.one;

        private void OnDrawGizmos/*Selected*/()
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    if ( (x+y) % 2 == 0)
                    {
                        Gizmos.color = new Color(0.29f, 0.47f, 1f, 0.6f);
                    }
                    else
                    {
                        Gizmos.color = new Color(0.24f, 1f, 0.22f, .6f);
                    }

                    Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
                }
            }
        }
    }
}
