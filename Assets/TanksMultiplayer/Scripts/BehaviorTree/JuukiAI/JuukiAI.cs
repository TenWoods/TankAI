using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{

    public class JuukiAI : BaseControl
    {
        // 攻击范围(避免特殊子弹浪费)
        private float attackRadius = 12.0f;
        // 子弹躲避范围
        private float avoidRadius = 5.0f;
        // 根节点
        private SelectNode rootNode;
        // 拾取生命动作
        private GetItem getHealth;
        // 拾取护盾动作
        private GetItem getShield;
        // 拾取子弹动作
        private GetItem getBullet;
        // 攻击血量最低敌人动作
        private AttackEnemyLowHealth attackLow;
        private int attackNormalNum = 4;
        // 攻击范围内敌人动作
        private List<NearAttack> attackNormals;
        // 分数最高时攻击下一位动作
        private AttackNextScore attackNext;
        // 攻击比自己分数低的玩家动作
        private AttackLowerScore attackLowerScore;
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
        // 时间过半条件
        private IsGameHalf gameHalf;
        private HasBulletPower hasBullet;
        // 分数最高条件
        private IsAtTop atTop;
        // 最低分条件
        private IsLast atLast;
        // 下位存在条件
        private IsNextAlive nextAlive;
        // 选择节点数量
        private int seletNum = 2;
        // 所有选择节点
        private List<SelectNode> selectNodes;
        // 顺序节点数量
        private int sequenceNum = 6;
        // 所有顺序节点
        private List<SequenceNode> sequenceNodes;
        // 并行节点数量
        private int parallelNum = 4;
        // 所有并行节点
        private List<ParallelNode> parallelNodes;

        protected override void OnInit()
        {
            InitNodes();
            InitBehaviorTree();
        }

        protected override void OnStop()
        {
            ClearState();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            NodeState result = rootNode.Execute();
        }

        /// <summary>
        /// 初始化所有节点
        /// </summary>
        private void InitNodes()
        {
            rootNode = new SelectNode();
            getHealth = new GetItem(tankPlayer, "Health", avoidRadius);
            getShield = new GetItem(tankPlayer, "Shield", avoidRadius);
            getBullet = new GetItem(tankPlayer, "BulletPower", avoidRadius);
            attackLow = new AttackEnemyLowHealth(tankPlayer, attackRadius, avoidRadius);
            attackNormals = new List<NearAttack>();
            attackNext = new AttackNextScore(tankPlayer, attackRadius, avoidRadius);
            attackLowerScore = new AttackLowerScore(tankPlayer, attackRadius, avoidRadius);
            for (int i = 0; i < attackNormalNum; i++)
            {
                attackNormals.Add(new NearAttack(tankPlayer, attackRadius));
            }
            danger = new IsDanger(tankPlayer);
            healthExist = new IsItemExist("Health");
            hasShield = new HasShield(tankPlayer);
            hasBullet = new HasBulletPower(tankPlayer);
            shieldExist = new IsItemExist("Shield");
            bulletExtist = new IsItemExist("BulletPower");
            gameHalf = new IsGameHalf();
            atTop = new IsAtTop(tankPlayer);
            atLast = new IsLast(tankPlayer);
            nextAlive = new IsNextAlive(tankPlayer);
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
            //第一层
            rootNode.AddNode(sequenceNodes[0]);
            rootNode.AddNode(sequenceNodes[1]);
            rootNode.AddNode(sequenceNodes[2]);
            rootNode.AddNode(selectNodes[0]);
            //第二层
            sequenceNodes[0].AddNode(danger);
            sequenceNodes[0].AddNode(healthExist);
            sequenceNodes[0].AddNode(parallelNodes[0]);
            sequenceNodes[1].AddNode(hasShield);
            sequenceNodes[1].AddNode(shieldExist);
            sequenceNodes[1].AddNode(parallelNodes[1]);
            sequenceNodes[2].AddNode(hasBullet);
            sequenceNodes[2].AddNode(bulletExtist);
            sequenceNodes[2].AddNode(parallelNodes[2]);
            selectNodes[0].AddNode(sequenceNodes[3]);
            selectNodes[0].AddNode(parallelNodes[3]);
            //第三层
            parallelNodes[0].AddNode(getHealth);
            parallelNodes[0].AddNode(attackNormals[0]);
            parallelNodes[1].AddNode(getShield);
            parallelNodes[1].AddNode(attackNormals[1]);
            parallelNodes[2].AddNode(getBullet);
            parallelNodes[2].AddNode(attackNormals[2]);
            sequenceNodes[3].AddNode(gameHalf);
            sequenceNodes[3].AddNode(selectNodes[1]);
            parallelNodes[3].AddNode(attackLow);
            parallelNodes[3].AddNode(attackNormals[3]);
            //第四层
            selectNodes[1].AddNode(sequenceNodes[4]);
            selectNodes[1].AddNode(sequenceNodes[5]);
            //第五层
            sequenceNodes[4].AddNode(atTop);
            sequenceNodes[4].AddNode(nextAlive);
            sequenceNodes[4].AddNode(attackNext);
            sequenceNodes[5].AddNode(atLast);
            sequenceNodes[5].AddNode(attackLowerScore);
        }

        private void ClearState()
        {
            rootNode.LastRunNode = null;
            foreach (var node in sequenceNodes)
            {
                node.LastRunNode = null;    
            }
            foreach (var node in selectNodes)
            {
                node.LastRunNode = null;
            }
        }
    }
}