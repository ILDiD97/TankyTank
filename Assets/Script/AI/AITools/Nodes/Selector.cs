using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : IBehaviourNode
{
    private List<IBehaviourNode> nodes;

    public Selector(params IBehaviourNode[] nodes)
    {
        this.nodes = new List<IBehaviourNode>(nodes);
    }

    public bool Execute()
    {
        foreach (IBehaviourNode node in nodes)
        {
            if (node.Execute())
            {
                return true;
            }
        }
        return false;
    }
}
