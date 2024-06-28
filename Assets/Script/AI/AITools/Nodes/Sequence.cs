using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : IBehaviourNode
{
    private List<IBehaviourNode> nodes;

    public Sequence(params IBehaviourNode[] nodes)
    {
        this.nodes = new List<IBehaviourNode>(nodes);
    }

    public bool Execute()
    {
        foreach (IBehaviourNode node in nodes)
        {
            if (!node.Execute())
            {
                return false;
            }
        }
        return true;
    }


}
