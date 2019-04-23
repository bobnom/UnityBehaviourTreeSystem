using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

/// <summary>
/// Robert Ward
/// Behavior tree for bandits
/// </summary>

public class BanditBehaviourTree  {

    Root root = new Root();

    MoveAction moveToPlayer;
    MoveAction moveToPoint;
    ActionNode moveToCover;
    ThisOrThat aiIsAgro;

    //Idle Nodes
    Sequence patrol = new Sequence();

    //Combat nodes
    Selector combat = new Selector();
    Sequence attackSequence = new Sequence();
    Sequence firingSequence = new Sequence();
    Sequence reloadSequence = new Sequence();
    Sequence takeCoverSequence = new Sequence();
    Sequence combatMovement = new Sequence();
    //Not sure if I want to keep it this way. This needs to decide when to go to cover
    ThisOrThat hasHighHealth;

    SetVector calcCoverPos;
    ConditionalAction canFire;
    ActionNode reload;
    ActionNode fire;

	public BanditBehaviourTree(BanditAIBehaviour bandit)
    {
        moveToPlayer = new MoveAction(bandit.Move, bandit.FindPlayer);
        moveToPoint = new MoveAction(bandit.Move, bandit.GetCurrentPatrolPoint);
        moveToCover = new ActionNode(bandit.MoveToCover);
        patrol.OpenBranch(
           moveToPoint,
          new ActionNode(bandit.HasReachedPoint)
           );

        fire = new ActionNode(bandit.Attack);
        reload = new ActionNode(bandit.Reload);

        ConditionalAction playerInView = new ConditionalAction(bandit.PlayerInView);

        firingSequence.OpenBranch(
            new ConditionalAction(bandit.PlayerInRange), 
            playerInView, 
            new ThisOrThat(fire, reload, bandit.CanFire));

        combatMovement.OpenBranch(
           new Inverter(playerInView),
            new ThisOrThat(moveToPlayer, moveToCover, bandit.HasHighHealth)
           
            );

  
        combat.OpenBranch(
            firingSequence,
            combatMovement
            );
        
        //TODO add functionality to calculate a cover position

        root.OpenBranch(new ThisOrThat(combat, patrol, bandit.IsAgro));
    }

    public void Update()
    {
        root.Tick();
    }
}
