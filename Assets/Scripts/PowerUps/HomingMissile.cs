using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public GameObject target;
    public float speed = 25f;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            Destroy(target);
            Destroy(gameObject);
        }
    }
}
