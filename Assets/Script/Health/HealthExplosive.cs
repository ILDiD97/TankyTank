using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HealthExplosive : MonoBehaviour, IDamageable
{
    public ParticleSystem explosive;

    public AudioSource explosion;

    public int health;

    public void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            if(explosive != null)
            {
                explosive.Play();
                explosion.PlayOneShot(explosion.clip);
            }
            Destroy(GetComponent<Collider>());
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(DestroyAfter());
        }
    }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
