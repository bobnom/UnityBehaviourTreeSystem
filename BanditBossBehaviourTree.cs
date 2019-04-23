using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

[System.Serializable]
public class BanditBossBehaviourTree {

    Root treeRoot;

    Sequence whistleAndRetreat = new Sequence();
    Sequence throwMolotov = new Sequence();
    Sequence fireShotgun = new Sequence();
    Sequence move = new Sequence();
    Sequence healthEventSeq = new Sequence();
    Sequence attack = new Sequence();

    Selector findBestAction = new Selector();

    MoveAction moveToPlayer;
    MoveAction moveToThrowingPosition;
    MoveAction moveToHidingPlace;

    ActionNode healthEvent;
    ActionNode whistle;
    ActionNode hide;

    public BanditBossBehaviourTree(BanditBossBehaviour boss)
    {
        moveToPlayer = new MoveAction(boss.Move, boss.FindPlayer);
        moveToThrowingPosition = new MoveAction(boss.Move, boss.PositionForHealthEvent);
        moveToHidingPlace = new MoveAction(boss.Move, boss.HidingPlace);

        fireShotgun.OpenBranch(new ActionNode(boss.ShotgunAttack));
        throwMolotov.OpenBranch(new ActionNode(boss.Attack));
        ThisOrThat selectAttack = new ThisOrThat(throwMolotov, fireShotgun, boss.PlayerOnLowerLevel);

        healthEvent = new ActionNode(boss.HealthEvent);

        //TODO Define both weapon sequences and whistle sequence

        whistleAndRetreat.OpenBranch(
            whistle = new ActionNode(boss.WhistleToAllies),
            moveToHidingPlace,
            hide = new ActionNode(boss.Hide)
            );

        healthEventSeq.OpenBranch(
            new ConditionalAction(boss.HealthEventDue),
            moveToThrowingPosition,
            healthEvent,
            whistleAndRetreat
            );

        attack.OpenBranch(
            moveToPlayer,
            selectAttack
            );

        findBestAction.OpenBranch(
            healthEventSeq,
            attack
            );
        treeRoot = new Root();
        treeRoot.OpenBranch(findBestAction);
    }

    public void Update()
    {
        treeRoot.Tick();
    }
}
