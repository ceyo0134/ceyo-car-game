using System.Collections;
using UnityEngine;
using TMPro;

// Player objesine eklenecek script. 1 anda 1 kart taşır (Mario Kart tarzı).
// Dikkat: Bu oyunda rakip araba yok (tek kişilik parkur oyunu), bu yüzden
// "saldırı" kartları (Dalga/Tuzak/İtiş/Mermi/Güdüm) rakip yerine
// parkurdaki engelleri (Tag: "Bumper") ve antiboost alanlarını (Tag: "Bo_down") hedefler.
[RequireComponent(typeof(Move))]
public class PowerUpInventory : MonoBehaviour
{
    [Header("Ayarlar")]
    public KeyCode activateKey = KeyCode.Space;
    public float effectRadius = 8f;
    public float forwardRange = 15f;
    public LayerMask obstacleLayer = ~0; // boş bırakılırsa tüm layer'larda tag ile arar

    [Header("UI (opsiyonel)")]
    public TMP_Text currentPowerUpText;

    public bool HasPowerUp => hasPowerUp;

    private bool hasPowerUp = false;
    private PowerUpType current;
    private Move move;

    private static readonly string[] ObstacleTags = { "Bumper", "Bo_down" };

    void Awake()
    {
        move = GetComponent<Move>();
        UpdateUI();
    }

    void Update()
    {
        if (hasPowerUp && Input.GetKeyDown(activateKey))
        {
            Activate(current);
            hasPowerUp = false;
            UpdateUI();
        }
    }

    public void GivePowerUp(PowerUpType type)
    {
        current = type;
        hasPowerUp = true;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentPowerUpText == null) return;
        currentPowerUpText.text = hasPowerUp ? PowerUpData.Get(current).DisplayName : "-";
    }

    private void Activate(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Nitro:
                move.ActivateNitro(boostAmount: 15f, extraMaxSpeed: 20f, duration: 2.5f);
                break;

            case PowerUpType.Kalkan:
                move.ActivateShield(4f);
                break;

            case PowerUpType.Onarim:
                move.ClearSlowEffect();
                break;

            case PowerUpType.Dalga:
                DisableNearbyObstacles(effectRadius, 3f);
                break;

            case PowerUpType.Itis:
                DisableNearbyObstacles(effectRadius * 0.6f, 6f, destroy: true);
                break;

            case PowerUpType.Tuzak:
                DropTrap();
                break;

            case PowerUpType.Mermi:
                StartCoroutine(FireVolley(3, 0.25f));
                break;

            case PowerUpType.Gudum:
                FireHoming();
                break;
        }
    }

    // ---- Dalga / İtiş: yakındaki engelleri geçici devre dışı bırakır ya da yok eder ----
    private void DisableNearbyObstacles(float radius, float disableSeconds, bool destroy = false)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, obstacleLayer);
        foreach (var hit in hits)
        {
            if (!HasObstacleTag(hit.gameObject)) continue;

            if (destroy)
            {
                Destroy(hit.gameObject);
            }
            else
            {
                var disabler = hit.gameObject.GetComponent<ObstacleTempDisabler>();
                if (disabler == null) disabler = hit.gameObject.AddComponent<ObstacleTempDisabler>();
                disabler.DisableFor(disableSeconds);
            }
        }
    }

    // ---- Tuzak: arkaya, üzerine gelince yavaşlatan basit bir tuzak bırakır ----
    private void DropTrap()
    {
        Vector3 pos = transform.position - transform.forward * 2f + Vector3.up * 0.1f;
        GameObject trap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trap.transform.position = pos;
        trap.transform.localScale = new Vector3(1.2f, 0.05f, 1.2f);
        trap.name = "CeyoTuzak";

        var col = trap.GetComponent<Collider>();
        col.isTrigger = true;

        var hazard = trap.AddComponent<TrapHazard>();
        hazard.slowAmount = 6f;

        Destroy(trap, 20f); // bir süre sonra kendiliğinden kaybolur
    }

    // ---- Mermi: ileri doğru 3 atış, her biri yoldaki ilk engeli temizler ----
    private IEnumerator FireVolley(int shots, float interval)
    {
        for (int i = 0; i < shots; i++)
        {
            TryClearForwardObstacle();
            yield return new WaitForSeconds(interval);
        }
    }

    // ---- Güdüm: en yakın engele kilitlenip yok eden mermi ----
    private void FireHoming()
    {
        GameObject target = FindNearestObstacle();
        if (target == null) return;

        GameObject missile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        missile.transform.position = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
        missile.transform.localScale = Vector3.one * 0.4f;
        missile.name = "CeyoGudum";

        var col = missile.GetComponent<Collider>();
        col.isTrigger = true;

        var homing = missile.AddComponent<HomingMissile>();
        homing.target = target;

        Destroy(missile, 6f);
    }

    private void TryClearForwardObstacle()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, forwardRange, obstacleLayer))
        {
            if (HasObstacleTag(hit.collider.gameObject))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private GameObject FindNearestObstacle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, forwardRange, obstacleLayer);
        GameObject nearest = null;
        float best = float.MaxValue;
        foreach (var hit in hits)
        {
            if (!HasObstacleTag(hit.gameObject)) continue;
            Vector3 toHit = hit.transform.position - transform.position;
            if (Vector3.Dot(toHit.normalized, transform.forward) < 0.2f) continue; // sadece önündekiler
            float dist = toHit.sqrMagnitude;
            if (dist < best)
            {
                best = dist;
                nearest = hit.gameObject;
            }
        }
        return nearest;
    }

    private bool HasObstacleTag(GameObject go)
    {
        foreach (var tag in ObstacleTags)
        {
            if (go.CompareTag(tag)) return true;
        }
        return false;
    }
}
