using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private FactoryBase factoryBase;

    [SerializeField]
    private ParticleSystem fire;

    public void Damage(int damage)
    {
        factoryBase.Health -= damage;
        
        GameStatus.Instance.CalculateFacrotyHealth();
        if(factoryBase.Health <= 0)
        {
            Destroy(GetComponent<Collider>());
            fire.Play();
            StartCoroutine(StartDestroing());
            factoryBase.StopAllCoroutines();
        }
    }

    private IEnumerator StartDestroing()
    {
        yield return new WaitForSeconds(2);
        Destroy(factoryBase.gameObject);
        NavigationBuilder[] builders =
            FindObjectsByType<NavigationBuilder>(FindObjectsSortMode.InstanceID);
        foreach (NavigationBuilder navigation in builders)
        {
            navigation.Refresh();
        }
    }


}
