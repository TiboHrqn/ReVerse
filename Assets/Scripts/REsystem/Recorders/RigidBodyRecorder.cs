using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RigidBodyRecorder : ActiveComponentRecorder
{
    public Rigidbody rigidBody { get; private set; }

    public Vector3 velocity { get; private set; }
    public bool isKinematic { get; private set; }

    public RigidBodyRecorder(Rigidbody rb)
    {
        this.rigidBody = rb;
        this.velocity = rb.velocity;
        isKinematic = rb.isKinematic;
    }


    public override bool IsStateDifferent()
    {
        if(velocity != rigidBody.velocity || isKinematic != rigidBody.isKinematic)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void EvenRecorderState()
    {
        velocity = rigidBody.velocity;
        isKinematic = rigidBody.isKinematic;
    }

    protected override void EvenComponentState()
    {
        rigidBody.velocity = velocity;
        rigidBody.isKinematic = isKinematic;

    }


    protected override ComponentState GetRecorderState()
    {
        return new RigidBodyState(velocity, isKinematic);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is RigidBodyState rigidBodyState)
        {
            velocity = rigidBodyState.velocity;
            isKinematic = rigidBodyState.isKinematic;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if(stateA is RigidBodyState rbStateA && stateB is RigidBodyState rbStateB)
        {
            Vector3 newVelocity = Vector3.Lerp(rbStateA.velocity, rbStateB.velocity, p);
            return new RigidBodyState(newVelocity, rbStateA.isKinematic);
        }
        else 
        {
            throw new Exception("Wrong component state type");
        }
    }


    public override void DisableActiveComponent()
    {
        rigidBody.isKinematic = true;
    }

    public override void EnableActiveComponent()
    {
        rigidBody.isKinematic = false;
    }

    
}

public class RigidBodyState : ComponentState
{
    public Vector3 velocity { get; private set; }
    public bool isKinematic { get; private set; }

    public RigidBodyState(Vector3 velocity, bool isKinematic)
    {
        this.velocity = velocity;
        this.isKinematic = isKinematic;
    }
}
