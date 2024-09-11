using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LineRendererRecorder : ComponentRecorder //pour l'instant ne tient compte que du Material, regarder le fonctionnement des MaterialPropertyBlock pour +
{
    public LineRenderer lineRenderer { get; private set; }

    public Material material { get; private set; }
    public Vector3[] positions { get; private set; }

    //rajouter newPositions ici s'il faut optimiser le code

    public LineRendererRecorder(LineRenderer lineRend)
    {
        lineRenderer = lineRend;
        material = lineRend.material;
        positions = new  Vector3[lineRend.positionCount];
        lineRend.GetPositions(positions);
    }

    public override bool IsStateDifferent()
    {
        Vector3[] newPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(newPositions);
        if( material != lineRenderer.material || positions != newPositions )
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
        material = lineRenderer.material;
        positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
    }

    protected override void EvenComponentState()
    {
        lineRenderer.material = material;
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    protected override ComponentState GetRecorderState()
    {
        return new LineRendererState(material, positions);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is LineRendererState lineRendState)
        {
            material = lineRendState.material;
            int size = lineRendState.positions.Length;
            positions = new Vector3[size];
            Array.Copy(lineRendState.positions, positions, size);
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if (stateA is LineRendererState lineRendStateA && stateB is LineRendererState lineRendStateB)
        {
            return lineRendStateA;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }
}

public class LineRendererState : ComponentState
{
    public Material material { get; private set; }
    public Vector3[] positions { get; private set; }

    public LineRendererState(Material material, Vector3[] pos)
    {
        this.material = material;
        int size = pos.Length;
        positions = new Vector3[size];
        Array.Copy(pos, positions, size);

    }
}