using UnityEngine;
using System.Collections;

public interface Buildable {
    
    int getStage();
    string getProperty();
    void switchValue();
    void applyStage();
    bool hasNextStage();
    bool canBeBuilt();

    void pressMouse(Vector3 pressPosition);
    void moveMouse(Vector3 movePosition);
    void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition);
    void dragMouse(Vector3 pressedPosition, Vector3 dragPosition);
}
