using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimatorRecorder : ActivePausableComponentRecorder
{
    public Animator animator { get; private set; }

    public int currentAnimationHash { get; private set; }
    public float normalizedTime { get; private set; }
    public Dictionary<string, float> floatParameters { get; private set; }
    public Dictionary<string, int> intParameters { get; private set; }
    public Dictionary<string, bool> boolParameters { get; private set; }

    public AnimatorRecorder(Animator animator)
    {
        this.animator = animator;
    }

    public override bool IsStateDifferent()
    {
        if (!animator || !animator.runtimeAnimatorController || !animator.isActiveAndEnabled)
        {
            if (currentAnimationHash == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isDifferent = currentAnimationHash != stateInfo.shortNameHash ||
                           Mathf.Abs(normalizedTime - stateInfo.normalizedTime) > Mathf.Epsilon;

        if (!isDifferent)
        {
        foreach (AnimatorControllerParameter param in animator.parameters)
            {
                switch (param.type)
                {   
                    case AnimatorControllerParameterType.Float:
                        if (floatParameters[param.name] != animator.GetFloat(param.name))
                            return true;
                        break;
                    case AnimatorControllerParameterType.Int:
                        if (intParameters[param.name] != animator.GetInteger(param.name))
                            return true;
                        break;
                    case AnimatorControllerParameterType.Bool:
                        if (boolParameters[param.name] != animator.GetBool(param.name))
                            return true;
                        break;
                }
            }
        }

        return isDifferent;
    }

    protected override void EvenRecorderState()
    {
        if (!animator || !animator.runtimeAnimatorController || !animator.isActiveAndEnabled)
        {
            currentAnimationHash = 0;
            normalizedTime = 0f;
            floatParameters = new Dictionary<string, float>();
            intParameters = new Dictionary<string, int>();
            boolParameters = new Dictionary<string, bool>();
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool isPlaying = animator.HasState(0, stateInfo.shortNameHash) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0;

        currentAnimationHash = isPlaying ? stateInfo.shortNameHash : 0;
        normalizedTime = isPlaying ? stateInfo.normalizedTime : 0f;

        floatParameters = new Dictionary<string, float>();
        intParameters = new Dictionary<string, int>();
        boolParameters = new Dictionary<string, bool>();

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
        switch (param.type)
            {
            case AnimatorControllerParameterType.Float:
                floatParameters[param.name] = animator.GetFloat(param.name);
                break;
            case AnimatorControllerParameterType.Int:
                intParameters[param.name] = animator.GetInteger(param.name);
                break;
            case AnimatorControllerParameterType.Bool:
                boolParameters[param.name] = animator.GetBool(param.name);
                break;
            }
        }

        ComponentState state = new AnimatorState(currentAnimationHash, normalizedTime, floatParameters, intParameters, boolParameters);
    }

    protected override void EvenComponentState()
{
    if (floatParameters == null || intParameters == null || boolParameters == null)
    {
        return;
    }

    foreach (var param in floatParameters)
    {
        animator.SetFloat(param.Key, param.Value);
    }

    foreach (var param in intParameters)
    {
        animator.SetInteger(param.Key, param.Value);
    }

    foreach (var param in boolParameters)
    {
        animator.SetBool(param.Key, param.Value);
    }

    if (currentAnimationHash != 0)
    {
        animator.Play(currentAnimationHash, 0, normalizedTime);
        animator.speed = 1f;
    }
}

    protected override ComponentState GetRecorderState()
    {
        return new AnimatorState(currentAnimationHash, normalizedTime, floatParameters, intParameters, boolParameters);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is AnimatorState animatorState)
        {
            currentAnimationHash = animatorState.currentAnimationHash;
            normalizedTime = animatorState.normalizedTime;
            floatParameters = animatorState.floatParameters;
            intParameters = animatorState.intParameters;
            boolParameters = animatorState.boolParameters;

            if (currentAnimationHash == 0)
            {
                animator.enabled = false;
            }
            else
            {
                animator.enabled = true;
            }
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if (stateA is AnimatorState animStateA && stateB is AnimatorState animStateB)
        {
            if (animStateA.currentAnimationHash != animStateB.currentAnimationHash)
            {
                return animStateA;
            }
            else
            {
            var newFloatParams = new Dictionary<string, float>();
            var newIntParams = new Dictionary<string, int>();
            var newBoolParams = new Dictionary<string, bool>();

            foreach (var param in animStateA.floatParameters)
            {
                newFloatParams[param.Key] = Mathf.Lerp(param.Value, animStateB.floatParameters[param.Key], p);
            }

            foreach (var param in animStateA.intParameters)
            {
                newIntParams[param.Key] = Mathf.RoundToInt(Mathf.Lerp(param.Value, animStateB.intParameters[param.Key], p));
            }

            foreach (var param in animStateA.boolParameters)
            {
                newBoolParams[param.Key] = (p < 0.5f) ? param.Value : animStateB.boolParameters[param.Key];
            }

            return new AnimatorState(animStateA.currentAnimationHash, Mathf.Lerp(animStateA.normalizedTime, animStateB.normalizedTime, p),
                                     newFloatParams, newIntParams, newBoolParams);

            }
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    public override void DisableActiveComponent()
    {
        animator.enabled = false;
    }

    public override void EnableActiveComponent()
    {
        animator.enabled = true;
    }

    public override void PauseComponent()
    {
        animator.speed = 0f;
    }
}

public class AnimatorState : ComponentState
{
    public int currentAnimationHash { get; private set; }
    public float normalizedTime { get; private set; }
    public Dictionary<string, float> floatParameters { get; private set; }
    public Dictionary<string, int> intParameters { get; private set; }
    public Dictionary<string, bool> boolParameters { get; private set; }

    public AnimatorState(int currentAnimationHash, float normalizedTime,
                        Dictionary<string, float> floatParameters,
                        Dictionary<string, int> intParameters,
                        Dictionary<string, bool> boolParameters) 
    {
        this.currentAnimationHash = currentAnimationHash;
        this.normalizedTime = normalizedTime;
        this.floatParameters = floatParameters;
        this.intParameters = intParameters;
        this.boolParameters = boolParameters; 
    }

    public override string GetTrace()
    {
        return "";
    }
}
