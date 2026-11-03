using System.Collections;
using UnityEngine;

// AI rakip araba. Tag = "Opponent" olmalı (Player'la karışmasın ve
// güç kartları onu doğru tespit edebilsin).
public class AIDriver : MonoBehaviour
{
    [Header("Parkur")]
    public WaypointPath path;
    public int currentWaypoint = 0;
    public float waypointReachDistance = 3f;

    [Header("Sürüş")]
    public float maxSpeed = 22f;
    public float acceleration = 10f;
    public float turnSpeed = 90f;

    [Header("Zorluk (oyuncuya göre ayarlama - opsiyonel)")]
    public Transform player;
    public bool rubberBanding = false;
    public float rubberBandStrength = 4f; // geride kalınca hız bonusu, öndeyken ceza

    private float currentSpeed = 0f;
    private bool isSpinning = false;
    private float slowMultiplier = 1f;

    void Update()
    {
        if (path == null || path.Count == 0) return;

        if (isSpinning)
        {
            // Savrulma sırasında kontrolsüzce dönüyor, ilerlemiyor.
            transform.Rotate(0, 360f * Time.deltaTime, 0);
            return;
        }

        Transform target = path.GetPoint(currentWaypoint);
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0;

        if (toTarget.magnitude < waypointReachDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= path.Count)
            {
                if (path.loop) currentWaypoint = 0;
                else currentWaypoint = path.Count - 1; // son noktada dur
            }
            return;
        }

        // Hedefe doğru dön
        Quaternion lookRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

        // Hız
        float targetSpeed = maxSpeed * slowMultiplier * RubberBandFactor();
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    private float RubberBandFactor()
    {
        if (!rubberBanding || player == null) return 1f;

        float distAlongTrack = Vector3.Distance(transform.position, player.position);
        // Basit yaklaşım: oyuncudan çok uzaktaysa (geride kaldıysa) hızlan, öndeyse yavaşla.
        // Daha gelişmiş bir sistem checkpoint index'i karşılaştırmalı; bu basit bir başlangıçtır.
        bool aiBehindPlayer = Vector3.Dot(player.position - transform.position, transform.forward) > 0;
        if (aiBehindPlayer) return 1f + Mathf.Clamp01(distAlongTrack / 50f) * (rubberBandStrength * 0.1f);
        return 1f - Mathf.Clamp01(distAlongTrack / 50f) * (rubberBandStrength * 0.05f);
    }

    // --- Güç kartlarının çağırdığı metodlar ---

    public void ApplySlow(float multiplier, float duration)
    {
        StartCoroutine(SlowRoutine(multiplier, duration));
    }

    private IEnumerator SlowRoutine(float multiplier, float duration)
    {
        slowMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        slowMultiplier = 1f;
    }

    public void SpinOut(float duration)
    {
        StartCoroutine(SpinRoutine(duration));
    }

    private IEnumerator SpinRoutine(float duration)
    {
        isSpinning = true;
        currentSpeed = 0f;
        yield return new WaitForSeconds(duration);
        isSpinning = false;
    }
}
