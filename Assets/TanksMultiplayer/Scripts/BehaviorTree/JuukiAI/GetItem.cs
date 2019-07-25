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
        private List<GameObject> allItems;
        // 目标关键字
        private string keyword;
        private float avoidRadius; 

        public GetItem(BasePlayer player, string keyword, float avoidRadius) : base()
        {
            this.player = player;
            this.keyword = keyword;
            allItems = new List<GameObject>();
            this.avoidRadius = avoidRadius;
            FindAllItems();
        }

        public override NodeState Execute()
        {
            Debug.Log("GetItem" + keyword);
            FindAllItems();
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
            if (Vector3.Magnitude(player.transform.position - target.transform.position) >= 1.0f)
            {
                result = NodeState.Success;
                Move();
                return result;
            }
            if (!target.activeInHierarchy)
            {
                target = null;
                result = NodeState.Success;
                return result;
            }
            else
            {
                target = null;
                result = NodeState.Fail;
                return result;
            }
        }
        
        private void Move()
        {
            GameObject bullet = null;
            Vector3 direction = Vector3.zero;
            bullet = FindBulletAround();
            if (bullet == null)
            {
                player.MoveTo(target.transform.position);
                return;
            }
            Vector3 bulletDir = Vector3.Normalize(bullet.GetComponent<Rigidbody>().velocity);
            direction = Quaternion.AngleAxis(90, Vector3.up) * bulletDir;
            for (int i = 0; i < 5; i++)
            {
                player.SimpleMove(new Vector2(direction.x, direction.z));
            }
            Debug.Log("Avoid");
        }

        /// <summary>
        /// 探测附近子弹
        /// </summary>
        /// <returns></returns>
        private GameObject FindBulletAround()
        {
            GameObject bullet = null;
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].GetComponent<Bullet>().owner.GetComponent<BasePlayer>().teamIndex == player.teamIndex)
                {
                    continue;
                }
                if (Vector3.Magnitude(bullets[i].transform.position - player.transform.position) <= avoidRadius)
                {
                    Vector3 bulletDir = Vector3.Normalize(bullets[i].GetComponent<Rigidbody>().velocity);
                    float angle = Vector3.Angle(Vector3.Normalize(player.transform.forward), bulletDir);
                    if (angle < 45.0f || angle > 135.0f)
                    {
                        bullet = bullets[i];
                        return bullet;
                    }
                }
            }
            return bullet;
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
        private void FindAllItems()
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