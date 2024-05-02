using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RobberCloak : RobberMoveSet
{
    [SerializeField] RobberAbility ability;
    float cloacktime;
    public override void AbilityUpdate()
    {
        if (Input.GetKeyDown(ability.Button))
        {
            if (cloacktime > Time.time) return;
            ability.Activate();
            cloacktime = Time.time + ability.CoolDown;
        }
    }
}
