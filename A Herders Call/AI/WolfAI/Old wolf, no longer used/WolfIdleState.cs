using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wolf/WolfIdleState")]
public class WolfIdleState : WolfBaseState
{
    // Start is called before the first frame update
    public override void Enter()
    {
        owner.StartCoroutine(waiter());

    }

    public override void Update()
    {
        if (WolfPack.prayFound)
            owner.TransitionTo<WolfChaseState>();
        base.Update();
    }

    private IEnumerator waiter(){

        yield return new WaitForSeconds(GetWaitTime());
        owner.TransitionTo<WolfPatrolState>();
    }
}
