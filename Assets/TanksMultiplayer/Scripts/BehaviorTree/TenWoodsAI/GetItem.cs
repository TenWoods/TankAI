using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
/// <summary>
    /// 获取道具
    /// </summary>
    public class GetItem : ActionNode
    {
        private GameObject target = null;
        private BasePlayer player;
        // 场上所有目标道具
        List<GameObject> allItems;
        // 目标关键字
        string keyword;


        public GetItem(BasePlayer player, string keyword) : base()
        {
            this.player = player;
            this.keyword = keyword;
            allItems = new List<GameObject>();
            FindAllItems();
        }

        public override NodeState Execute()
        {
            NodeState result = NodeState.Fail;
            if (target == null)
            {
                if (!FindBestTarget())
                {
                    result = NodeState.Fail;
                    return result;
                }
            }
            if (!target.activeInHierarchy)
            {
                target = null;
                result = NodeState.Fail;
                return result;
            }
            if (Vector3.Magnitude(player.transform.position - target.transform.position) >= 0.1f)
            {
                result = NodeState.Running;
                player.MoveTo(target.transform.position);
                target = null;
                return result;
            }
            result = NodeState.Success;
            return result;
        }
        
        /// <summary>
        /// 寻找离自己最近的道具
        /// </summary>
        private bool FindBestTarget()
        {
            float minDistanace = 10000.0f;
            float tempDistance = 0.0f;
            int minIndex = -1;
            for (int i = 0; i < allItems.Count; i++)
            {
                tempDistance = Vector3.Magnitude(allItems[i].transform.position - player.transform.position);
                if (tempDistance < minDistanace && allItems[i].activeInHierarchy)
                {
                    minDistanace = tempDistance;
                    minIndex = i;
                }
            }
            if (minIndex != -1)
            {
                target = allItems[minIndex];
                return true;
            }
            else
            {
                // 当前场上没有可以使用的道具
                return false;
            }
        }

        /// <summary>
        /// 找到场上所有的加血道具
        /// </summary>
        public void FindAllItems()
        {
            allItems.Clear();
            GameObject[] items = GameObject.FindGameObjectsWithTag("Powerup");
            string itemName;
            //Debug.Log(items.Length);
            foreach (var item in items)
            {
                itemName = item.name;
                if (itemName.Contains(keyword))
                {
                    allItems.Add(item);
                }
            }
        }

        public GameObject Target 
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
    }
}