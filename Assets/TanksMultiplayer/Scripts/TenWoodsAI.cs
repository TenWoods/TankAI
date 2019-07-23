using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{

    public class TenWoodsAI : BaseControl
    {
        private GameObject targetItem = null;
        private BasePlayer targetPlayer = null;
        private SelectNode rootNode = new SelectNode();

        protected override void OnInit()
        {
            
            InitBehaviorTree();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (targetItem == null)
            {
                GameObject[] allItem = GameObject.FindGameObjectsWithTag("Powerup");
                string name;
                int i = 0;
                for (i = 0; i < allItem.Length; i++)
                {
                    name = allItem[i].name;
                    if(name.Contains("Shield"))
                    {
                        break;
                    }
                }
                targetItem = allItem[i];
            }
            if (targetItem != null)
            {
                tankPlayer.MoveTo(targetItem.transform.position);
            }
        }

        private void InitBehaviorTree()
        {
            Debug.Log("InitTree");
        }
    }


    public class GetItem : ActionNode
    {
        private GameObject target = null;
        private BasePlayer player;

        public GetItem(GameObject target, BasePlayer player) : base()
        {
            this.target = target;
            this.player = player;
        }

        public override NodeState Execute()
        {
            return NodeState.Fail;
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