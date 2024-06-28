using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalDecorator : IBehaviourNode
{
    private IBehaviourNode conditionNode;
    private IBehaviourNode childNode;

    public ConditionalDecorator(IBehaviourNode conditionNode, IBehaviourNode childNode)
    {
        this.conditionNode = conditionNode;
        this.childNode = childNode;
    }

    public bool Execute()
    {
        if (conditionNode.Execute())
        {
            return childNode.Execute();
        }
        return false;
    }
}
