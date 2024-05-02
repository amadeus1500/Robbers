using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class RobberController : PlayerController
{
   public RobberMoveSet moveset;
    private void Update()
    {
        moveset.AbilityUpdate();
    }
}
