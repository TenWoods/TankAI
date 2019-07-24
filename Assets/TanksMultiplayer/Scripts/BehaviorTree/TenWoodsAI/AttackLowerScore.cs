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

        public AttackLowerScore(BasePlayer player) : base()
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
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
            player.MoveTo(target.transform.position);
            if (player.bShootable)
            {
                player.AimAndShoot(target.transform.position);
            }
            target = null;
            return result;
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
