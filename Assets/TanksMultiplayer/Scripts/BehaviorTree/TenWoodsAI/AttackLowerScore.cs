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
            FindAllPlayers();
            NodeState result = NodeState.Fail;
            if (target == null)
            {

            }
        }

        private bool FindLowerPlayer()
        {
            
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
