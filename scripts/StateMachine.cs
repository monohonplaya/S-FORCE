using Godot;
using System;
using System.Collections.Generic;
public class StateMachine : Node
{
    // May want to use a state stack instead of these 2 variables
    public String _state { get; private set; } = null;
    public String _previousState { get; private set; } = null;
    private Dictionary<String, int> states = new Dictionary<String, int>();
    /* public override void _PhysicsProcess(float delta)
    {
        if (_state != null)
        {
            StateLogic(delta);
            String transition =  GetTransition(delta);
            if (transition != null)
                SetState(transition);
        }
    }
    protected void StateLogic(float delta)
    {
        return;
    } */
    public String GetTransition(float delta)
    {
        return null;
    }
    public void EnterState(String newState, String oldState)
    {

    }
    public void ExitState(String oldState, String newState)
    {

    }
    public void SetState(String newState)
    {
        _previousState = _state;
        _state = newState;/* 
        if (_previousState != null)
            ExitState(_previousState, newState);
        if (newState != null)
            EnterState(newState, _previousState); */
    }   

    public void AddState(String stateName)
    {
        states.Add(stateName, states.Keys.Count);
    }
}