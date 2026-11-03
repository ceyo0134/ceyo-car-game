using System.Collections;
using UnityEngine;

// Bir engeli belirli süreliğine görünmez ve dokunulmaz yapar, sonra geri getirir.
// Dalga/İtiş kartları tarafından çalışma anında eklenir.
public class ObstacleTempDisabler : MonoBehaviour
{
    public void DisableFor(float seconds)
    {
        StartCoroutine(Routine(seconds));
    }

    private IEnumerator Routine(float seconds)
    {
        Collider col = GetComponent<Collider>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (col != null) col.enabled = false;
        foreach (var r in renderers) r.enabled = false;

        yield return new WaitForSeconds(seconds);

        if (col != null) col.enabled = true;
        foreach (var r in renderers) r.enabled = true;

        Destroy(this);
    }
}
