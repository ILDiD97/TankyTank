using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthNormal : MonoBehaviour, IDamageable
{
    [SerializeField]
    private AudioSource destroyAudio;

    [SerializeField]
    private int health;
    public void Damage(int damage)
    {
        health -= damage;
        if(health < 0)
        {
            Destroy(GetComponent<Collider>());
            destroyAudio.PlayOneShot(destroyAudio.clip);
            StartCoroutine(DestroyAfter());
        }
    }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(0);
        Destroy(gameObject);
    }
}
