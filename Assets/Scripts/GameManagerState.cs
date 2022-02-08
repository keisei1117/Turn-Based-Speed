using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameManagerにstateパターンを適用
abstract class GameManagerState
{
    abstract public void update();
}

class 