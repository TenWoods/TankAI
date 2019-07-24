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

        public AttackNextScore(BasePlayer player) : base()
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
            FindAllPlayers();
        }

        public override NodeState Execute()
        {
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
            player.MoveTo(nextPlayer.transform.position);
            if (player.bShootable)
            {
                player.AimAndShoot(nextPlayer.transform.position);
            }
            nextPlayer = null;
            return result;
        }

        private bool FindNextPlayer()
        {
            int nextTeamIndex = -1;
            int nextScore = 0;
            List<int> scores = new List<int>(GameManager.GetInstance().score);
            for (int i = 0; i < scores.Count; i++)
            {
                if (i != player.teamIndex && scores[i] > nextScore)
                {
                    nextScore = scores[i];
                    nextTeamIndex = i;
                }
            }
            if (scores[nextTeamIndex] > scores[player.teamIndex])
            {
                return false;
            }
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (allPlayers[i].teamIndex == nextTeamIndex)
                {
                    nextPlayer = allPlayers[i];
                    break;
                }
            }
            return true;
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
