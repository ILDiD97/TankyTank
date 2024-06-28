using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BehaviourTree : Singleton<BehaviourTree>
{

    public virtual async void TreeRootUpdate(IBehaviourNode root)
    {
        root.Execute();
        await Task.Yield();
    }
}
