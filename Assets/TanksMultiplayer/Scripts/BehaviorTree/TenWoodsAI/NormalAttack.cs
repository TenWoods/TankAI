using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
    public class NearAttack : ActionNode
    {
        private List<BasePlayer> allPlayersInRange;
        private BasePlayer player;
        private BasePlayer target = null;
        private float attackRadius;

        public NearAttack(BasePlayer player, float attackRadius) : base()
        {
            this.player = player;
            this.attackRadius = attackRadius;
            allPlayersInRange = new List<BasePlayer>();
            FindAllPlayers();
        }

        public override NodeState Execute()
        {
            Debug.Log("?");
            FindAllPlayers();
            NodeState result = NodeState.Fail;
            if (target == null)
            {
                if (!FindTargetInRange())
                {
                    result = NodeState.Fail;
                    return result;
                }
            }
            result = NodeState.Success;
            if (player.bShootable)
            {
                player.AimAndShoot(target.transform.position);
            }
            target = null;
            return result;
        }

        private bool FindTargetInRange()
        {
            int index = -1;
            int minHealth = 15;
            int tempHealth = 0;
            if (allPlayersInRange.Count <= 0)
            {
                return false;
            }
            // 寻找范围内血量最少并且没有护盾的
            for (int i = 0; i < allPlayersInRange.Count; i++)
            {
                tempHealth = allPlayersInRange[i].health;
                if (allPlayersInRange[i].shield <= 0 && tempHealth < minHealth)
                {
                    minHealth = tempHealth;
                    index = i;
                }
            }
            // 全有护盾情况下攻击护盾最少的
            if (index != -1)
            {
                target = allPlayersInRange[index];
                return true;
            }
            int minShield = 15;
            int tempShield = 0;
            for (int i = 0; i < allPlayersInRange.Count; i++)
            {
                tempShield = allPlayersInRange[i].shield;
                if (tempShield <= minShield)
                {
                    minShield = tempShield;
                    index = i;
                }
            }
            if (index != -1)
            {
                target = allPlayersInRange[index];
                return true;
            }
            return false;
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

        private void FindAllPlayers()
        {
            allPlayersInRange.Clear();
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float distance = 0;
            foreach(var p in players)
            {
                var comp = p.GetComponent<BasePlayer>();
                if (comp.teamIndex != player.teamIndex)
                {
                    distance = Vector3.Magnitude(comp.transform.position - player.transform.position);
                    if (distance <= attackRadius)
                    {
                        allPlayersInRange.Add(comp);
                    }
                }
            }
        }
    }   
}