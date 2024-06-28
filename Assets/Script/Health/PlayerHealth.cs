using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public PlayerController movement;

    public AudioSource fire;
    public AudioClip explosion;

    public ParticleSystem flames;

    public void Damage(int damage)
    {
        movement.Health -= damage;
        UIManager.Instance.UIHealth(movement.Health);
        if(movement.Health == 0)
        {
            fire.PlayOneShot(explosion);
            fire.Play();
            flames.Play();
            GameStatus.Instance.SetStartGame(true);
            UIManager.Instance.Lost();
        }
    }
}
