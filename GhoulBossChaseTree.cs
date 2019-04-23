using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class GhoulBossChaseTree  {
    Root root = new Root();
    
    public Sequence huntKill = new Sequence();

    public MoveAction moveToPlayer;

    public GhoulBossChaseTree(GhoulBossAIBehaviour gBoss)
    {
        moveToPlayer = new MoveAction(gBoss.Move, gBoss.FindPlayer);

        huntKill.OpenBranch(
            //new SetVector(goul.FindPlayer, moveToPlayer.SetTarget), 
            moveToPlayer,
            new ConditionalAction(gBoss.PlayerInRange),
            new ActionNode(gBoss.MeleeAttack));
        root.OpenBranch(huntKill);
    }

    internal void Tick()
    {
        root.Tick();
    }
}
