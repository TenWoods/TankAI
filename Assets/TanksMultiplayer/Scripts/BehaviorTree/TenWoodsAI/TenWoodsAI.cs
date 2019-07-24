using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{

    public class TenWoodsAI : BaseControl
    {
        private float attackRadius = 10.0f;
        // 根节点
        private SelectNode rootNode;
        // 拾取生命动作
        private GetItem getHealth;
        // 拾取护盾动作
        private GetItem getShield;
        // 拾取子弹动作
        private GetItem getBullet;
        // 攻击血量最低敌人动作
        private AttackEnemyWithLowHealth attackLow;
        private int attackNormalNum = 3;
        // 攻击范围内敌人动作
        private List<NearAttack> attackNormals;
        // 危险条件
        private IsDanger danger;
        // 回血道具存在条件
        private IsItemExist healthExist;
        // 拥有护盾条件
        private HasShield hasShield;
        // 护盾道具存在条件
        private IsItemExist shieldExist;
        // 子弹道具存在条件
        private IsItemExist bulletExtist;
        // 选择节点数量
        private int seletNum = 1;
        // 所有选择节点
        private List<SelectNode> selectNodes;
        // 顺序节点数量
        private int sequenceNum = 3;
        // 所有顺序节点
        private List<SequenceNode> sequenceNodes;
        // 并行节点数量
        private int parallelNum = 3;
        // 所有并行节点
        private List<ParallelNode> parallelNodes;

        protected override void OnInit()
        {
            InitNodes();
            InitBehaviorTree();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!tankPlayer.IsAlive)
            {
                ClearState();
            }
            NodeState result = rootNode.Execute();
        }

        /// <summary>
        /// 初始化所有节点
        /// </summary>
        private void InitNodes()
        {
            rootNode = new SelectNode();
            getHealth = new GetItem(tankPlayer, "Health");
            getShield = new GetItem(tankPlayer, "Shield");
            getBullet = new GetItem(tankPlayer, "BulletPower");
            attackLow = new AttackEnemyWithLowHealth(tankPlayer);
            attackNormals = new List<NearAttack>();
            for (int i = 0; i < attackNormalNum; i++)
            {
                attackNormals.Add(new NearAttack(tankPlayer, attackRadius));
            }
            danger = new IsDanger(tankPlayer);
            healthExist = new IsItemExist("Health");
            hasShield = new HasShield(tankPlayer);
            shieldExist = new IsItemExist("Shield");
            bulletExtist = new IsItemExist("BulletPower");
            selectNodes = new List<SelectNode>();
            for (int i = 0; i < seletNum; i++)
            {
                selectNodes.Add(new SelectNode());
            }
            sequenceNodes = new List<SequenceNode>();
            for (int i = 0; i < sequenceNum; i++)
            {
                sequenceNodes.Add(new SequenceNode());
            }
            parallelNodes = new List<ParallelNode>();
            for (int i = 0; i < parallelNum; i++)
            {
                parallelNodes.Add(new ParallelNode());
            }
        }

        /// <summary>
        /// 生成树结构
        /// </summary>
        private void InitBehaviorTree()
        {
            rootNode.AddNode(sequenceNodes[0]);
            rootNode.AddNode(sequenceNodes[1]);
            rootNode.AddNode(sequenceNodes[2]);

            sequenceNodes[0].AddNode(danger);
            sequenceNodes[0].AddNode(healthExist);
            sequenceNodes[0].AddNode(parallelNodes[0]);
            parallelNodes[0].AddNode(getHealth);
            parallelNodes[0].AddNode(attackNormals[0]);

            sequenceNodes[1].AddNode(hasShield);
            sequenceNodes[1].AddNode(shieldExist);
            sequenceNodes[1].AddNode(parallelNodes[1]);
            parallelNodes[1].AddNode(getShield);
            parallelNodes[1].AddNode(attackNormals[1]);

            sequenceNodes[2].AddNode(bulletExtist);
            sequenceNodes[2].AddNode(parallelNodes[2]);
            parallelNodes[2].AddNode(getBullet);
            parallelNodes[2].AddNode(attackNormals[2]);

            rootNode.AddNode(attackLow);
        }

        private void ClearState()
        {
            rootNode.LastRunNode = null;
            foreach (var node in sequenceNodes)
            {
                node.LastRunNode = null;    
            }
        }
    }
}