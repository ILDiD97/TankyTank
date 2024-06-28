using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;

    public string spawnerName;

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable health = other.GetComponent<IDamageable>();
        if (health != null && other.tag != spawnerName)
        {
            health.Damage(damage);
        }
        if (other.tag != "Base")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
