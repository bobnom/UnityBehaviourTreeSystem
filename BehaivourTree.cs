using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public enum NodeState
    {
        Failed,
        Succeeded,
        Running
    }

    public class BehaivourTree
    {
        public static ActionNode MoveTo(System.Func<NodeState> MoveTo) { return new ActionNode(MoveTo); }
        public static Sequence sequence(params TreeNode[] children) { Sequence seq = new Sequence(); seq.OpenBranch(children); return seq; }
        
    }

    public abstract class TreeNode
    {
        public NodeState State;
        
        public abstract NodeState Tick();
    }

    public abstract class Composite : TreeNode
    {
        protected int currentChild = 0;
        protected List<TreeNode> children = new List<TreeNode>();
        public virtual void OpenBranch(params TreeNode[] children)
        {
            foreach (TreeNode n in children)
            {
                this.children.Add(n);
            }
        }

        

        public List<TreeNode> GetChildren()
        {
            return children;
        }

    }

    public class Root : Composite
    {
        public override NodeState Tick()
        {
            children[currentChild].Tick();
           // MonoBehaviour.print("Root Child " + currentChild + " NodeState " + children[currentChild].State);
            switch (children[currentChild].State)
            {
                case NodeState.Succeeded:
                    currentChild++;
                    if (currentChild == children.Count)
                    {
                        currentChild = 0;
                        return NodeState.Succeeded;
                    }
                    else
                    {
                        return NodeState.Running;
                    }
                case NodeState.Failed:
                    currentChild = 0;
                    return NodeState.Failed;
                case NodeState.Running:
                    return NodeState.Running;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }

    public class Sequence : Composite
    {
        public override NodeState Tick()
        {
            children[currentChild].Tick();
          //  MonoBehaviour.print("Sequence current child " + currentChild + " " + children[currentChild].State);
            switch (children[currentChild].State)
            {
                case NodeState.Succeeded:
                    currentChild++;
                    
                    if(currentChild == children.Count)
                    {
                        State = NodeState.Succeeded;
                        currentChild = 0;
                        return NodeState.Succeeded;
                    }
                    else
                    {
                        State = NodeState.Running;
                        return NodeState.Running;
                    }
                case NodeState.Failed:
                    State = NodeState.Failed;
                    currentChild = 0;
                    return NodeState.Failed;
                case NodeState.Running:
                    State = NodeState.Running;
                    return NodeState.Running;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }


    
    public class Decorator : TreeNode
    {
        public override NodeState Tick()
        {
            throw new NotImplementedException();
        }
    }

    public class Inverter : Decorator
    {
        TreeNode node;

        public Inverter(TreeNode node)
        {
            this.node = node;
        }

        public override NodeState Tick()
        {
           switch(node.Tick())
            {
                case NodeState.Succeeded:
                    State = NodeState.Failed;
                    return NodeState.Failed;
                case NodeState.Failed:
                    State = NodeState.Succeeded;
                    return NodeState.Succeeded;
                case NodeState.Running:
                    State = NodeState.Running;
                    return NodeState.Running;
                default:
                    return NodeState.Failed;
            } 
        }
    }

    public class Selector : Composite
    {
        public override NodeState Tick()
        {
              children[currentChild].Tick();
         //   MonoBehaviour.print("Selector current child " + currentChild + " " + children[currentChild].State);
            switch (children[currentChild].State)
            {
                case NodeState.Failed:
                    currentChild++;
                    State = NodeState.Failed;
                    if (currentChild == children.Count)
                    {
                        currentChild = 0;
                        return NodeState.Failed;
                    }
                    else
                    {
                        State = NodeState.Running;
                        return NodeState.Running;
                    }
                case NodeState.Running:
                    State = NodeState.Running;
                    return NodeState.Running;
                case NodeState.Succeeded:
                    State = NodeState.Succeeded;
                    currentChild = 0;
                    return NodeState.Succeeded;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }

    public class ThisOrThat : Composite
    {

        protected TreeNode a;
        protected TreeNode b;
        protected System.Func<bool> test;

        public ThisOrThat(TreeNode a, TreeNode b, System.Func<bool> test)
        {
            this.a = a;
            this.b = b;
            this.test = test;
        }

        public override NodeState Tick()
        {
            if(test())
            {
                State = a.Tick();
            }
            else
            {
                State = b.Tick();
            }
            switch(State)
            {
                case NodeState.Succeeded:
                    return NodeState.Succeeded;
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failed:
                    return NodeState.Failed;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }

    public abstract class Leaf : TreeNode
    {
       
    }

 

    public class WaitNode : Leaf
    {
        const float WaitForFixedUpdate = 0.02f;
        float time;
        float currentTime;
        Func<float, NodeState> action;
        public WaitNode(System.Func<float, NodeState> action, float time)
        {
            this.time = time;
            currentTime = time;
            this.action = action;
        }

        public override NodeState Tick()
        {
            currentTime -= WaitForFixedUpdate;

            switch (action(currentTime))
            {
                case NodeState.Succeeded:
                    currentTime = time;
                    State = NodeState.Succeeded;
                    return NodeState.Succeeded;
                case NodeState.Running:
                    State = NodeState.Running;
                    return NodeState.Running;
                case NodeState.Failed:
                    State = NodeState.Failed;
                    return NodeState.Failed;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }

    public class MoveAction : Leaf
    {
        System.Func<Vector3,NodeState> action;
        System.Func<Vector3> target;
        public MoveAction(System.Func<Vector3, NodeState> action,System.Func<Vector3> target)
        {
            this.action = action;
            this.target = target;
        }

        //public void SetTarget(Vector3 tar)
        //{
        //    target = tar;
        //}

        public override NodeState Tick()
        {
          //  MonoBehaviour.print(target);
            switch (action(target()))
            {
                case NodeState.Succeeded:
                    State = NodeState.Succeeded;
                    return NodeState.Succeeded;
                case NodeState.Running:
                    State = NodeState.Running;
                    return NodeState.Running;
                case NodeState.Failed:
                    State = NodeState.Failed;
                    return NodeState.Failed;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }

    public class ActionNode : Leaf
    {
        System.Func<NodeState> action;
        //  private NodeState nodeState;

        public ActionNode(System.Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Tick()
        {
            switch(action())
            {
                case NodeState.Succeeded:
                    State = NodeState.Succeeded;
                    return NodeState.Succeeded;
                case NodeState.Running:
                    State = NodeState.Running;
                    return NodeState.Running;        
                case NodeState.Failed:
                    State = NodeState.Failed;
                    return NodeState.Failed;
                default:
                    throw new System.Exception("This shouldn't happen but it has, you should rethink a few things");
            }
        }
    }
    public class ConditionalAction : Leaf
    {
        System.Func<bool> condition;

        public ConditionalAction(System.Func<bool> condition)
        {
            this.condition = condition;
        }

        public override NodeState Tick()
        {
            if(condition())
            {
                State = NodeState.Succeeded;
                return NodeState.Succeeded;
            }
            else
            {
                State = NodeState.Failed;
                return NodeState.Failed;
            }

          
        }
    }

    //Possibly needless
    public class SetVector : TreeNode
    {
        System.Func<Vector3> set;
        System.Action<Vector3> toBeSet;

        public SetVector(System.Func<Vector3> set, System.Action< Vector3> toBeSet)
        {
            this.toBeSet = toBeSet;
            this.set = set;
           
        }

        public override NodeState Tick()
        {
          
                toBeSet(set());
                   
                    State = NodeState.Succeeded;
                    return NodeState.Succeeded;
             
        }
    }

   
}



