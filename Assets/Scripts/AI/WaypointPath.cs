using UnityEngine;

// Boş bir GameObject oluştur (ör. "LEVEL_1_Path"), altına sırayla boş child GameObject'ler
// koy (her biri parkurun bir noktası) ve bu script'i parent'a ekle.
// AIDriver bu objeyi referans alarak child'ları sırayla takip eder.
public class WaypointPath : MonoBehaviour
{
    [Tooltip("Boş bırakılırsa bu objenin tüm child'ları sırayla waypoint kabul edilir.")]
    public Transform[] points;

    public bool loop = false;

    void Awake()
    {
        if (points == null || points.Length == 0)
        {
            points = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                points[i] = transform.GetChild(i);
            }
        }
    }

    public int Count => points != null ? points.Length : 0;

    public Transform GetPoint(int index)
    {
        if (points == null || points.Length == 0) return null;
        if (loop) index = ((index % points.Length) + points.Length) % points.Length;
        else index = Mathf.Clamp(index, 0, points.Length - 1);
        return points[index];
    }
}
