using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberAbility : MonoBehaviour
{
    public string Name;
    public float CoolDown;
    public KeyCode Button;
    public RobberMoveSet moveset;
    public virtual void Activate()
    {

    }
}
