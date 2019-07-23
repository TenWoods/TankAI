using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{

    public class TenWoodsAI : BaseControl
    {
        private float attackRadius = 6;
        private SelectNode rootNode;
        private GetItem getHealth;
        private AttackEnemyWithLowHealth attackLow;
        protected override void OnInit()
        {
            rootNode = new SelectNode();
            InitBehaviorTree();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            GameObject[] items = GameObject.FindGameObjectsWithTag("Powerup");
            //getHealth.FindAllItems(); 
            attackLow.FindAllPlayers();
            NodeState result = rootNode.Execute();
            //Debug.Log(result);
        }

        private void InitBehaviorTree()
        {
            Debug.Log("InitTree");
            //getHealth = new GetItem(tankPlayer, "Health");
            attackLow = new AttackEnemyWithLowHealth(tankPlayer);
            //rootNode.AddNode(getHealth);
            rootNode.AddNode(attackLow);
        }
    }
}