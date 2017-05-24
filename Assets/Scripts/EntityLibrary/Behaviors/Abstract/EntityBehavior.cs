using BehaviorLibrary;
using BehaviorLibrary.Components.Composites;
using System.Collections;
using System.Collections.Generic;

public abstract class EntityBehavior {

    RootSelector root; //declare the root
    Behavior _behavior; //declare the behavior

    public abstract Behavior behavior
    {
        get;
    }

    //this is used to declare the behavior and root selector
    public abstract void initialize();

    //this is used to tick the behavior tree
    public abstract void behave();
}
