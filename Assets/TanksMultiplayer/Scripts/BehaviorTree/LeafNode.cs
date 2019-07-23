using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 叶节点父类
    /// </summary>
    public abstract class LeafNode : Node
    {
        public LeafNode(NodeType type) : base(type)
        {
            
        }
    }

    /// <summary>
    /// 条件叶节点
    /// </summary>
    public abstract class ConditionNode : LeafNode
    {
        public ConditionNode() : base(NodeType.Condition)
        {
            
        }
    }

    /// <summary>
    /// 行为叶节点
    /// </summary>
    public abstract class ActionNode : LeafNode
    {
        public ActionNode() : base(NodeType.Action)
        {

        }
    }
}