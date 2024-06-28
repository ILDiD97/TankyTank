using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private FactoryBase factoryBase;
    public void Damage(int damage)
    {
        factoryBase.Health -= damage;
        GameStatus.Instance.CalculateFacrotyHealth();
    }
}
