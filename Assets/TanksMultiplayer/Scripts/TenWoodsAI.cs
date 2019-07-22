using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksMP
{
    public class TenWoodsAI : BaseControl
    {
        GameObject targetItem = null;
        BasePlayer targetPlayer = null;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (targetItem == null)
            {
                GameObject[] allItem = GameObject.FindGameObjectsWithTag("Powerup");
                targetItem = allItem[Random.Range(0, allItem.Length)];
            }
            if (targetItem != null)
            {
                tankPlayer.MoveTo(targetItem.transform.position);
            }
        }
    }
}