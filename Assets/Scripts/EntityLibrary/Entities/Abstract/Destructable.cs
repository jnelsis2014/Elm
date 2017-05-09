using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Destructable : World {

    public abstract float integrity
    {
        get;
    }
}
