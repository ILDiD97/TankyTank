using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour, IDamageable
{
    public AIPlayerSearch aI;

    public AudioSource fire;
    public AudioClip explosion;

    public ParticleSystem flames;

    public void Damage(int damage)
    {
        aI.Health -= damage;
        if(aI.Health == 0)
        {
            UIManager.Instance.UIScore();
            fire.PlayOneShot(explosion);
            fire.Play();
            flames.Play();
            aI.Dead = true;
            aI.DeadSequence();
        }
        GameStatus.Instance.CalculateEnemyHealth();
    }
}
