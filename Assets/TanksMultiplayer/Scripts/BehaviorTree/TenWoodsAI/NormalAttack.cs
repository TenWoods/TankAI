using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
    public class NormalAttack : ActionNode
    {
        public override NodeState Execute()
        {
            return NodeState.Fail;
        }
    }   
}