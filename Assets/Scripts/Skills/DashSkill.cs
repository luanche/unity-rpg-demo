using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : Skill
{
    public override void useSkill()
    {
        base.useSkill();

        Debug.Log("Created clone behind");
    }
}
