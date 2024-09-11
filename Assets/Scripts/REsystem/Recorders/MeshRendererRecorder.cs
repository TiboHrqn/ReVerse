using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshRendererRecorder : ComponentRecorder //pour l'instant ne tient compte que du Material, regarder le fonctionnement des MaterialPropertyBlock pour +
{
    public MeshRenderer meshRenderer { get; private set; }

    public Material material { get; private set; }

    public MeshRendererRecorder(MeshRenderer rend)
    {
        this.meshRenderer = rend;
        this.material = rend.material;
    }

    public override bool IsStateDifferent()
    {
        return material != meshRenderer.material;
    }

    protected override void EvenRecorderState()
    {
        material = meshRenderer.material;
    }

    protected override void EvenComponentState()
    {
        meshRenderer.material = material;
    }

    protected override ComponentState GetRecorderState()
    {
        return new MeshRendererState(material);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is MeshRendererState meshRendState)
        {
            material = meshRendState.material;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if (stateA is MeshRendererState meshRendStateA && stateB is MeshRendererState meshRendStateB)
        {
            return new MeshRendererState(meshRendStateA.material);
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }
}

public class MeshRendererState : ComponentState
{
    public Material material { get; private set; }

    public MeshRendererState(Material material)
    {
        this.material = material;
    }
}