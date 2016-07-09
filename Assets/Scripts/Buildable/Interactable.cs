using UnityEngine;
using System.Collections;

public interface Interactable
{
    void startInteracting(AIBase interactor, int index);
    void stopInteracting(AIBase interactor, int index);
}
