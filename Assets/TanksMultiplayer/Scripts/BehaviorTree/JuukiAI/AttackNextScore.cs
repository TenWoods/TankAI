using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
    public class AttackNextScore : ActionNode
    {
        private BasePlayer player;
        private BasePlayer nextPlayer;
        private List<BasePlayer> allPlayers;
        private float attackRadius;
        private float avoidRadius;


        public AttackNextScore(BasePlayer player, float radius, float avoidRadius) : base()
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
            this.attackRadius = radius;
            this.avoidRadius = avoidRadius;
            FindAllPlayers();
        }

        public override NodeState Execute()
        {
            Debug.Log("AttackNext");
            FindAllPlayers();
            NodeState result = NodeState.Fail;
            if (nextPlayer == null)
            {
                if (!FindNextPlayer())
                {
                    result = NodeState.Fail;
                    return result;
                }
            }
            result = NodeState.Success;
            Move();
            if (player.bShootable && Vector3.Magnitude(nextPlayer.transform.position - player.transform.position) < attackRadius)
            {
                player.AimAndShoot(CalculateAttackDir());
            }
            nextPlayer = null;
            return result;
        }

        private void Move()
        {
            GameObject bullet = null;
            Vector3 direction = Vector3.zero;
            bullet = FindBulletAround();
            if (bullet == null)
            {
                player.MoveTo(nextPlayer.transform.position);
                return;
            }
            Vector3 bulletDir = Vector3.Normalize(bullet.GetComponent<Rigidbody>().velocity);
            float r = Random.Range(-1, 1);
            if (r <= 0)
            {
                direction = Quaternion.AngleAxis(-90, Vector3.up) * bulletDir;
            }
            else
            {
                direction = Quaternion.AngleAxis(-90, Vector3.up) * bulletDir;
            }
            player.SimpleMove(new Vector2(direction.x, direction.z));
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

        /// <summary>
        /// 寻找下一名次玩家
        /// </summary>
        /// <returns></returns>
        private bool FindNextPlayer()
        {
            int nextTeamIndex = -1;
            int nextScore = 0;
            List<int> scores = new List<int>(GameManager.GetInstance().score);
            for (int i = 0; i < scores.Count; i++)
            {
                if (i != player.teamIndex && scores[i] > nextScore && allPlayers[i].shield <= 0)
                {
                    nextScore = scores[i];
                    nextTeamIndex = i;
                }
            }
            if (scores[nextTeamIndex] > scores[player.teamIndex])
            {
                return false;
            }
            foreach (var p in allPlayers)
            {
                if (p.teamIndex == nextTeamIndex)
                {
                    nextPlayer = p;
                    break;
                }
            }
            if (nextPlayer == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算提前量
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculateAttackDir()
        {
            Vector3 result;
            Vector3 targetPos = nextPlayer.transform.position;
            Vector3 playerPos = player.transform.position;
            float cosTheta = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(player.transform.forward, nextPlayer.transform.forward));
            Vector3 direction = targetPos - playerPos;
            float distance = direction.magnitude;
            float a = -4.0625f;
            float b = -2.0f * distance * cosTheta;
            float c = distance * distance;
            float delta = Mathf.Pow(b, 2) - 4.0f * a * c;
            if (delta < 0)
            {   
                result = nextPlayer.transform.position;
            }
            else
            {
                float sqrtDelta = Mathf.Sqrt(delta);
                float x1 = (-b + sqrtDelta) / (2.0f * a);
                float x2 = (-b - sqrtDelta) / (2.0f * a);
                float x = Mathf.Min(x1, x2);
                if (x <= 0)
                {
                    result = nextPlayer.transform.position;
                }
                else
                {
                    result = targetPos + Vector3.Normalize(nextPlayer.transform.forward) * x;
                }
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
                allPlayers.Add(comp);
            }
        }
    }
}
