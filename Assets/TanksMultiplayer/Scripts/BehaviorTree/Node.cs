using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 节点当前状态
    /// </summary>
    public enum NodeState
    {
        Fail,
        Success,
        Running
    }

    public enum NodeType
    {
        Parallel,
        Select,
        Sequence,
        Decorate,
        RandomSelect,
        Condition,
        Action
    }

    /// <summary>
    /// 抽象节点类
    /// </summary>
    public abstract class Node
    {
        //节点在父节点处的索引
        protected int nodeIndex;
        protected NodeType nodeType;

        public Node(NodeType type)
        {
            nodeType = type;
        }

        public abstract NodeState Execute();

        public int NodeIndex
        {
            get
            {
                return nodeIndex;
            }
            set
            {
                nodeIndex = value;
            }
        }
    }

}
