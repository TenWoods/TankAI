using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
    public class AttackEnemyWithLowHealth : ActionNode
    {
        // 场上所有玩家
        private List<BasePlayer> allPlayers;
        private BasePlayer player;
        private BasePlayer target = null;

        public AttackEnemyWithLowHealth(BasePlayer player) : base()
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
            FindAllPlayers();
        }

        public override NodeState Execute()
        {
            Debug.Log("AttackLow");
            FindAllPlayers();
            NodeState result = NodeState.Fail;
            if (target == null)
            {
                if (!FindLowHealthPlayer())
                {
                    result = NodeState.Fail;
                    return result;
                }
            }
            result = NodeState.Success;
            player.MoveTo(target.transform.position);
            if (player.bShootable)
            {
                player.AimAndShoot(target.transform.position);
            }
            target = null;
            return result;
        }
        
        /// <summary>
        /// 寻找低血量敌人中最近的
        /// </summary>
        /// <returns></returns>
        private bool FindLowHealthPlayer()
        {
            int minIndex = -1;
            int minHealth = 15;
            int tempHealth = 0;
            float minDistance = 10000.0f;
            float tempDistance = 0.0f;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                tempHealth = allPlayers[i].health;
                if (tempHealth <= minHealth && tempHealth > 0)
                {
                    minHealth = tempHealth;
                    tempDistance = Vector3.Magnitude(allPlayers[i].transform.position - player.transform.position);
                    if (tempDistance < minDistance)
                    {
                        minDistance = tempDistance;
                        minIndex = i;
                    }
                }
            }
            if (minIndex != -1)
            {
                target = allPlayers[minIndex];
                return true;
            }
            else
            {
                return false;
            }
        }

        private Vector3 CalculateAttackDir()
        {
            Vector3 result;
            Vector3 targetPos = target.transform.position;
            Vector3 playerPos = player.transform.position;
            float cosTheta = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(player.Velocity, target.Velocity));
            Vector3 direction = targetPos - playerPos;
            float distance = direction.magnitude;
            float a = 1.0f - Mathf.Pow((18.0f / 8.0f), 2);
            float b = -2.0f * distance * cosTheta;
            float c = distance * distance;
            float delta = b * b - 4.0f * a * c;
            if (delta < 0)
            {   
                result = target.transform.position;
            }
            else
            {
                float sqrtDelta = Mathf.Sqrt(delta);
                float x1 = (-b + sqrtDelta) / (2.0f * a);
                float x2 = (-b - sqrtDelta) / (2.0f * a);
                float x = Mathf.Min(x1, x2);
                result = targetPos + Vector3.Normalize(target.Velocity) * x;
            }
            return result;
        }

        // 找到除自己之外所有的玩家
        private void FindAllPlayers()
        {
            allPlayers.Clear();
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(var p in players)
            {
                var comp = p.GetComponent<BasePlayer>();
                if (comp.teamIndex != player.teamIndex)
                {
                    allPlayers.Add(comp);
                }
            }
        }
    }   
}

