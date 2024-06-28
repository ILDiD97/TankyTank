using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUndestoyable : MonoBehaviour
{
    [SerializeField]
    private AudioSource destroyAudio;

    public void Damage(int damage)
    {
        destroyAudio.PlayOneShot(destroyAudio.clip);

    }

}
