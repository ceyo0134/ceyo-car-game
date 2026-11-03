using UnityEngine;

// Parkur üzerine yerleştirilecek güç kartı kutusu.
// Unity'de bu objeye Tag = "PowerUpBox" ve bir Collider (Is Trigger = true) vermeyi unutma.
public class PowerUpBox : MonoBehaviour
{
    [Tooltip("Boş bırakılırsa rastgele bir kart verilir.")]
    public bool randomType = true;
    public PowerUpType fixedType = PowerUpType.Nitro;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PowerUpInventory inventory = other.GetComponent<PowerUpInventory>();
        if (inventory == null) return;

        PowerUpType given = randomType
            ? (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length)
            : fixedType;

        inventory.GivePowerUp(given);
        Destroy(gameObject);
    }
}
