using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
    public class AttackLowerScore : ActionNode
    {
        private List<BasePlayer> allPlayers;
        private BasePlayer player;
        private BasePlayer target = null;
        private BasePlayer topPlayer;
        private float attackRadius;
        private float avoidRadius;

        public AttackLowerScore(BasePlayer player, float radius, float avoidRadius) : base()
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
            this.attackRadius = radius;
            this.avoidRadius = avoidRadius;
            FindAllPlayers();
        }
        
        public override NodeState Execute()
        {
            Debug.Log("AttackLower");
            FindAllPlayers();
            NodeState result = NodeState.Fail;
            if (target == null)
            {
                if (!FindLowerPlayer())
                {
                    result = NodeState.Fail;
                    return result;
                }
            }
            result = NodeState.Success;
            Move();
            if (player.bShootable && Vector3.Magnitude(target.transform.position - player.transform.position) < attackRadius)
            {
                player.AimAndShoot(CalculateAttackDir());
            }
            target = null;
            return result;
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
            player.SimpleMove(new Vector2(direction.x, direction.z));
            Debug.Log("Avoid");
        }

        private GameObject FindBulletAround()
        {
            GameObject bullet = null;
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            int minIndex = -1;
            float minDistance = 10000.0f;
            float distance = 0;
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].GetComponent<Bullet>().owner.GetComponent<BasePlayer>().teamIndex == player.teamIndex)
                {
                    continue;
                }
                distance = Vector3.Magnitude(bullets[i].transform.position - player.transform.position);
                if (distance <= avoidRadius)
                {
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minIndex = i;
                    }
                }
            }
            if (minIndex != -1)
            {
                bullet = bullets[minIndex];
            }
            return bullet;
        }

        private bool FindLowerPlayer()
        {
            int minIndex = -1;
            int minHealth = 15;
            int tempHealth = 0;
            List<int> lowerIndex = new List<int>();
            List<int> scores = new List<int>(GameManager.GetInstance().score);
            for (int i = 0; i < scores.Count; i++)
            {
                if (i != player.teamIndex && scores[i] < scores[player.teamIndex])
                {
                    lowerIndex.Add(i);
                }
            }
            for (int i = 0; i < allPlayers.Count; i++)
            {
                tempHealth = allPlayers[i].health;
                if (tempHealth <= minHealth && tempHealth > 0 && lowerIndex.Contains(allPlayers[i].teamIndex))
                {
                    minHealth = tempHealth;
                    minIndex = i;
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

        private void FindTopPlayer()
        {
            int maxIndex = -1;
            int maxScore = 0;
            List<int>scores = new List<int>(GameManager.GetInstance().score);
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (scores[allPlayers[i].teamIndex] > maxScore)
                {
                    maxScore = scores[allPlayers[i].teamIndex];
                    maxIndex = i;
                }
            }
            if (maxIndex != -1 && maxIndex != player.teamIndex)
            {
                topPlayer = allPlayers[maxIndex];
            }
        }

        /// <summary>
        /// 计算提前量
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculateAttackDir()
        {
            Vector3 result;
            Vector3 targetPos = target.transform.position;
            Vector3 playerPos = player.transform.position;
            float cosTheta = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(player.transform.forward, target.transform.forward));
            Vector3 direction = targetPos - playerPos;
            float distance = direction.magnitude;
            float a = -4.0625f;
            float b = -2.0f * distance * cosTheta;
            float c = distance * distance;
            float delta = Mathf.Pow(b, 2) - 4.0f * a * c;
            if (delta < 0)
            {   
                result = target.transform.position;
            }
            else
            {
                float sqrtDelta = Mathf.Sqrt(delta);
                float x1 = (-b + sqrtDelta) / (2.0f * a);
                float x2 = (-b - sqrtDelta) / (2.0f * a);
                float x = Mathf.Max(x1, x2);
                result = targetPos + Vector3.Normalize(target.transform.forward) * x;
            }
            return result;
        }

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
