using UnityEngine;
using System.Collections;

public interface Buildable {

    //List<BuildStage> getStages
    int getStage();
    string getProperty();
    void switchValue();
    void applyStage();
    bool hasNextStage();

    void pressMouse(Vector3 pressPosition);
    void moveMouse(Vector3 movePosition);
    void releaseMouse(Vector3 pressedPosition, Vector3 releasePosition);
    void dragMouse(Vector3 pressedPosition, Vector3 dragPosition);
}
