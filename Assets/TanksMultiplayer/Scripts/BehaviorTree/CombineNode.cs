using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 组合节点父类
    /// </summary>
    public abstract class CombineNode : Node
    {
        //子节点集
        protected List<Node> childNodes = new List<Node>();
        public CombineNode(NodeType type) : base(type)
        {

        }

        // 添加子节点
        public void AddNode(Node node)
        {
            int index = childNodes.Count;
            node.NodeIndex = index;
            childNodes.Add(node);
        }
    }

    /// <summary>
    /// 选择节点
    /// 或门逻辑:有Success即返回Success并终止后续操作;全为Fail时返回Fail;
    /// </summary>
    public class SelectNode : CombineNode
    {
        private Node lastRunNode = null;

        public SelectNode() : base(NodeType.Select)
        {

        }

        public override NodeState Execute()
        {
            int index = 0;
            if (lastRunNode != null)
            {
                index = lastRunNode.NodeIndex;
            }
            lastRunNode = null;
            NodeState result = NodeState.Fail;
            Node temp;
            for (int i = index; i < childNodes.Count; i++)
            {
                temp = childNodes[i];
                result = temp.Execute();
                if (result == NodeState.Success)
                {
                    break;
                }
                if (result == NodeState.Fail)
                {
                    continue;
                }
                if (result == NodeState.Running)
                {
                    lastRunNode = temp;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 顺序节点
    /// 与门逻辑:所有Success时返回Success;有Fail时立即返回Fail
    /// </summary>
    public class SequenceNode : CombineNode
    {
        private Node lastRunNode = null;

        public SequenceNode() : base(NodeType.Sequence)
        {

        }

        public override NodeState Execute()
        {
            int index = 0;
            if (lastRunNode != null)
            {
                index = lastRunNode.NodeIndex;
            }
            lastRunNode = null;
            NodeState result = NodeState.Fail;
            Node temp;
            for (int i = index; i < childNodes.Count; i++)
            {
                temp = childNodes[i];
                result = temp.Execute();
                if (result == NodeState.Fail)
                {
                    break;
                }
                if (result == NodeState.Success)
                {
                    continue;
                }
                if (result == NodeState.Running)
                {
                    lastRunNode = temp;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 随机选择节点
    /// </summary>
    public class RandomSelectNode : CombineNode
    {
        private Node lastRunNode;

        public RandomSelectNode() : base(NodeType.RandomSelect)
        {

        }

        public override NodeState Execute()
        {
            int index = -1;
            if (lastRunNode != null)
            {
                index = lastRunNode.NodeIndex;
            }
            lastRunNode = null;
            List<int> randomList = RandomIndex(childNodes.Count);
            NodeState result = NodeState.Fail;
            Node temp;
            while (randomList.Count > 0)
            {
                if (index < 0)
                {
                    index = randomList[randomList.Count - 1];
                    randomList.RemoveAt(randomList.Count - 1);
                }
                temp = childNodes[index];
                index = -1;
                result = temp.Execute();
                if (result == NodeState.Success)
                {
                    break;
                }
                if (result == NodeState.Fail)
                {
                    continue;
                }
                if (result == NodeState.Running)
                {
                    lastRunNode = temp;
                    break;
                }
            }
            return result;
        }

        // 获取一个随机序列
        private List<int> RandomIndex(int count)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < count; i++)
            {
                result.Add(i);
            }
            for (int i = 0; i < count; i++)
            {
                int j = i;
                while (j == i)
                {
                    j = Random.Range(0, count);
                }
                int temp = result[i];
                result[i] = result[j];
                result[j] = temp;
            }
            return result;
        }
    }

    public class ParallelNode : CombineNode
    {
        public ParallelNode() : base(NodeType.Parallel)
        {

        }

        public override NodeState Execute()
        {
            NodeState result = NodeState.Fail;
            int successCount = 0;
            Node temp;
            for (int i = 0; i < childNodes.Count; i++)
            {
                temp = childNodes[i];
                result = temp.Execute();
                if (result == NodeState.Fail)
                {
                    break;
                }
                if (result == NodeState.Success)
                {
                    successCount++;
                    continue;
                }
                if (result == NodeState.Running)
                {
                    continue;
                }
            }
            if (result != NodeState.Fail)
            {
                result = successCount >= childNodes.Count ? NodeState.Success : NodeState.Running;
            }
            return result;
        }
    }
}