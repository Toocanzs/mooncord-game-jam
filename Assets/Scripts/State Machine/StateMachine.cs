using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, BaseState> states;

    public BaseState CurrentState { get; private set; }
    public event Action<BaseState> OnStateChanged;

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        this.states = states;
    }

    void Update()
    {
        if (states == null || states.Values.Count <= 0)
            return;
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
            Debug.Log(CurrentState);
        }

        var nextState = CurrentState.Tick();
        if(nextState != null && nextState != CurrentState.GetType())
        {
            SwitchToNextState(nextState);
        }

    }

    private void SwitchToNextState(Type nextState)
    {
        CurrentState = states[nextState];
        OnStateChanged?.Invoke(CurrentState);
    }
}
