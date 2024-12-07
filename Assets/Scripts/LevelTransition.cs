using System;
using JetBrains.Annotations;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    public Action levelTransitionEvent;

    public void LevelTransitionAnimEvent()
    {
        levelTransitionEvent?.Invoke();
    }
}
