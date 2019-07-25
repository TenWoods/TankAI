using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace TanksMP
{
    /// <summary>
    /// 判断是否玩家拥有有护盾节点
    /// </summary>
    public class HasShield : ConditionNode
    {
        private BasePlayer player;

        public HasShield(BasePlayer player) : base()
        {
            this.player = player;
        }

        public override NodeState Execute()
        {
            if (player.shield > 0)
            {
                return NodeState.Fail;
            }
            else
            {
                return NodeState.Success;
            }
        }
    }

    /// <summary>
    /// 判断玩家是否拥有强力子弹
    /// </summary>
    public class HasBulletPower : ConditionNode
    {
        private BasePlayer player;
        
        public HasBulletPower(BasePlayer player)
        {
            this.player = player;
        }

        public override NodeState Execute()
        {
            if (player.currentBullet == 1)
            {
                return NodeState.Fail;
            }
            else
            {
                return NodeState.Success;
            }
        }

    }

    /// <summary>
    /// 判断场上是否有某种道具
    /// </summary>
    public class IsItemExist : ConditionNode
    {
        private string keyword;

        public IsItemExist(string keyword) : base()
        {
            this.keyword = keyword;
        }

        public override NodeState Execute()
        {
            List<GameObject> shield = new List<GameObject>();
            GameObject[] items = GameObject.FindGameObjectsWithTag("Powerup");
            string itemName;
            foreach (var item in items)
            {
                itemName = item.name;
                if (itemName.Contains(keyword))
                {
                    if (item.activeSelf)
                    {
                        shield.Add(item);
                    }
                }
            }
            if (shield.Count > 0)
            {
                return NodeState.Success;
            }
            else
            {
                return NodeState.Fail;
            }
            
        }
    }

    /// <summary>
    /// 判断玩家是否进入危险
    /// 条件为：小于指定血量或是小于最小血量
    /// </summary>
    public class IsDanger : ConditionNode
    {
        private BasePlayer player;
        private int dangerHealth = 5;
        private List<BasePlayer> allPlayers;

        public IsDanger(BasePlayer player) : base()
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
        }

        public override NodeState Execute()
        {
            FindAllPlayers();
            int lowestHealth = FindLowestHealth();
            if (player.health <= dangerHealth)
            {
                return NodeState.Success;
            }
            else
            {
                return NodeState.Fail;
            }
        }

        private int FindLowestHealth()
        {
            int minHealth = 15;
            int tempHealth = 0;
            foreach(var p in allPlayers)
            {
                tempHealth = p.health;
                if (tempHealth < minHealth)
                {
                    minHealth = tempHealth;
                }
            }
            return tempHealth;
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

    /// <summary>
    /// 游戏时间过半
    /// </summary>
    public class IsGameHalf : ConditionNode
    {
        public override NodeState Execute()
        {
            NodeState result = NodeState.Fail;
            float time = GameManager.GetInstance().GetTimeLeft();
            if (time <= 300)
            {
                result = NodeState.Success;
            }
            else
            {
                result = NodeState.Fail;
            }
            return result;
        }
    }

    /// <summary>
    /// 玩家最高分条件
    /// </summary>
    public class IsAtTop : ConditionNode
    {
        private BasePlayer player;

        public IsAtTop(BasePlayer player) : base()
        {
            this.player = player;
        }

        public override NodeState Execute()
        {
            int maxScore = 0;
            int maxIndex = -1;
            List<int> scores = new List<int>(GameManager.GetInstance().score);
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] > maxScore)
                {
                    maxScore = scores[i];
                    maxIndex = i;
                }
            }
            if (maxIndex == player.teamIndex)
            {
                return NodeState.Success;
            }
            else
            {
                return NodeState.Fail;
            }
        }
    }

    /// <summary>
    /// 判断玩家是否处于最末尾
    /// </summary>
    public class IsLast : ConditionNode
    {
        private BasePlayer player;

        public IsLast(BasePlayer player) : base()
        {
            this.player = player;
        }

        public override NodeState Execute()
        {
            int minScore = 1000;
            int minIndex = -1;
            List<int> scores = new List<int>(GameManager.GetInstance().score);
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] < minScore)
                {
                    minScore = scores[i];
                    minIndex = i;
                }
            }
            if (minIndex == player.teamIndex)
            {
                return NodeState.Fail;
            }
            else
            {
                return NodeState.Success;
            }
        }
    }

    /// <summary>
    /// 下位是否生存
    /// </summary>
    public class IsNextAlive : ConditionNode
    {
        private BasePlayer player;
        private List<BasePlayer> allPlayers;
        
        public IsNextAlive(BasePlayer player)
        {
            this.player = player;
            allPlayers = new List<BasePlayer>();
        }

        public override NodeState Execute()
        {
            BasePlayer nextPlayer = FindNextPlayer();
            if (nextPlayer != null && nextPlayer.IsAlive)
            {
                return NodeState.Success;
            }
            else
            {
                return NodeState.Fail;
            }
        }
        private BasePlayer FindNextPlayer()
        {
            BasePlayer nextPlayer = null;
            FindAllPlayers();
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
                return null;
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
                return null;
            }
            return nextPlayer;
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
