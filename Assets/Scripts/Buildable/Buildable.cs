using UnityEngine;
using System.Collections;
using System;

[Serializable]
public abstract class Buildable : DRectangle
{

    public enum MouseButton
    {
        Unknown,
        Left,
        Middle,
        Right
    }

    public override DRectangle Create(int x, int y, int width, int height) {
        return base.Create(x, y, width, height);
    }

    public abstract int getStage();
    public abstract string getProperty();
    public abstract void switchValue();
    public abstract void applyStage();
    public abstract void cancelStage();
    public abstract bool hasNextStage();
    public abstract bool canBeBuilt();

    public abstract void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition, MouseButton mouseButton);
    public abstract void pressMouse(Vector3 pressPosition, MouseButton mouseButton);
    public abstract void dragMouse(Vector3 pressedPosition, Vector3 dragPosition);
    public abstract void moveMouse(Vector3 movePosition);
}
