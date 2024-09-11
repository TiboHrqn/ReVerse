using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ImageRecorder : ComponentRecorder
{
    public Image image { get; private set; }

    public Sprite sprite { get; private set; }
    public Color color { get; private set; }

    public ImageRecorder(Image img)
    {
        this.image = img;
        this.sprite = img.sprite;
        this.color = img.color;
    }

    public override bool IsStateDifferent()
    {
        return sprite != image.sprite || color != image.color;
    }

    protected override void EvenRecorderState()
    {
        sprite = image.sprite;
        color = image.color;
    }

    protected override void EvenComponentState()
    {
        image.sprite = sprite;
        image.color = color;
    }

    protected override ComponentState GetRecorderState()
    {
        return new ImageState(sprite, color);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is ImageState imageState)
        {
            sprite = imageState.sprite;
            color = imageState.color;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if (stateA is ImageState imageStateA && stateB is ImageState imageStateB)
        {
            Color newColor = Color.Lerp(imageStateA.color, imageStateB.color, p);
            return new ImageState(imageStateA.sprite, newColor);
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }
}

public class ImageState : ComponentState
{
    public Sprite sprite { get; private set; }
    public Color color { get; private set; }

    public ImageState(Sprite sprite, Color color)
    {
        this.sprite = sprite;
        this.color = color;
    }

    public override string GetTrace()
    {
        return "{" + "\"Type\":\"Image\",\"Sprite\":\"" + sprite.name + "\",\"Color\":\"" + color.ToString() + "\"}";
    }
}
