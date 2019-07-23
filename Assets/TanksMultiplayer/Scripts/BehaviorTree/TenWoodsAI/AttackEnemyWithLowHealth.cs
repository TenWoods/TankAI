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
            NodeState result = NodeState.Fail;
            if (target == null)
            {
                if (!FindLowHealthPlayer())
                {
                    result = NodeState.Fail;
                    return result;
                }
            }
            if (!target.IsAlive)
            {
                target = null;
                result = NodeState.Success;
                return result;
            }
            if (Vector3.Magnitude(player.transform.position - target.transform.position) >= 2.0f)
            {
                result = NodeState.Running;
                player.MoveTo(target.transform.position);
                if (player.bShootable)
                {
                    player.AimAndShoot(CalculateAttackDir());
                }
                return result;
            }
            else
            {
                result = NodeState.Running;
                Vector3 dir = player.transform.position - target.transform.position;
                Vector3.Normalize(dir);
                player.SimpleMove(new Vector2(dir.x, dir.z));
                if (player.bShootable)
                {
                    player.AimAndShoot(CalculateAttackDir());
                }
                return result;
            }
        }
        
        /// <summary>
        /// 寻找最近的低血量敌人
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
            Vector3 result = new Vector3();
            Vector3 targetPos = target.transform.position;
            Vector3 playerPos = player.transform.position;
            float t = Vector3.Magnitude(targetPos - playerPos) / 18.0f;
            result = targetPos + t * target.agent.velocity;
            return result;
        }

        // 找到除自己之外所有的玩家
        public void FindAllPlayers()
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

