using UnityEngine;

public class TrapHazard : MonoBehaviour
{
    public float slowAmount = 6f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Move player = other.GetComponent<Move>();
        if (player != null)
        {
            player.ApplySlowEffect(slowAmount);
        }
        Destroy(gameObject);
    }
}
