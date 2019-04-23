using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
using System;

public class GhoulBossBehaviourTree {

    Root root;

    Selector determineAction = new Selector();

    MoveAction moveToPlayer;

    Sequence healthSequence = new Sequence();
    Sequence attackSequence = new Sequence();
    Sequence attackSequenceF = new Sequence();

    ActionNode prepareForAttack;
    ActionNode orbAttack;
    ActionNode aoeAttack;

    ActionNode orbAttackF;
    ActionNode aoeAttackF;

    ActionNode ascend;
    ActionNode decend;
    ActionNode healthEvent;

    internal void Tick()
    {
        root.Tick();
    }

    ConditionalAction becomeVulnerable;


    public GhoulBossBehaviourTree(GhoulBossAIBehaviour gBoss)
    {

        becomeVulnerable = new ConditionalAction(gBoss.beginHealthSequence);
        ascend = new ActionNode(gBoss.Ascend);
        decend = new ActionNode(gBoss.Decend);
        healthEvent = new ActionNode(gBoss.ActivateTotem);

        prepareForAttack = new ActionNode(gBoss.CoolDown);

        orbAttackF = new ActionNode(gBoss.AOEAttackFloating);
        aoeAttackF = new ActionNode(gBoss.OrbAttackFloating);

        attackSequenceF.OpenBranch(
            prepareForAttack,
            new ThisOrThat(orbAttackF, aoeAttackF, gBoss.PlayerInView)
            );

        healthSequence.OpenBranch(
            becomeVulnerable,
            ascend,
            attackSequenceF,
            decend,
            healthEvent
            );

        prepareForAttack = new ActionNode(gBoss.CoolDown);
        orbAttack = new ActionNode(gBoss.OrbAttack);
        aoeAttack = new ActionNode(gBoss.AOEAttack);

        attackSequence.OpenBranch(
            prepareForAttack,
            new ThisOrThat(orbAttack, aoeAttack, gBoss.PlayerInView)
            );

        determineAction.OpenBranch(
            healthSequence,
            attackSequence
            );

        root = new Root();
        root.OpenBranch(determineAction);
    }
	
}
