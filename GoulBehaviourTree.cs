using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

[System.Serializable]
public class GoulBehaviourTree
{
    // nodes for the tree
    public Root TreeRoot;
    public Selector main = new Selector();
    public Sequence huntKill = new Sequence();
    public Sequence patrol = new Sequence();
    public MoveAction moveToPlayer;
    public MoveAction moveToPoint;
    public ThisOrThat canSeePlayer;
  

    //I may move this elsewhere. it's used to direct the AI movement
    public Vector3 target;

    // I pass in the AI Behaviour to get access to the functions I 
    // need for the tree
    public GoulBehaviourTree(BaseAIBehaviour goul)
    {
        // First I build the nodes for the tree
      
        main = new Selector();
        moveToPlayer = new MoveAction(goul.Move, goul.FindPlayer);
        moveToPoint = new MoveAction(goul.Move, goul.GetNextPatrolPoint);

        huntKill.OpenBranch(
            //new SetVector(goul.FindPlayer, moveToPlayer.SetTarget), 
            moveToPlayer,
            new ConditionalAction(goul.PlayerInRange), 
            new ActionNode(goul.Attack));

  
        patrol.OpenBranch(
            //new SetVector(goul.GetNextPatrolPoint, moveToPoint.SetTarget), 
            moveToPoint, 
            new WaitNode(goul.WaitForSeconds, 7f)
           // new ConditionalAction(goul.HasReachedPoint)
            );

        canSeePlayer = new ThisOrThat(huntKill, patrol, goul.PlayerInView);

        main.OpenBranch(canSeePlayer);
        
        TreeRoot = new Root();
        TreeRoot.OpenBranch(main);
    }

    public void SetTarget(Vector3 tar)
    {
        target = tar;
    }
    public Vector3 GetTarget()
    {
        return  target;
    }
    public void Update()
    {
     //   MonoBehaviour.print(target);
        //update data in nodes before ticking
        //moveToPlayer.SetTarget(target);
        //moveToPoint.SetTarget(target);

        TreeRoot.Tick();
    }
    
}

