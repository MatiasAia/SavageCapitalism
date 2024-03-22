using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager control;
    public int moneyToCompleteLap = 500;
    public Transform[] checkpointsPositions;
    public Material[] carMaterials;
    // Start is called before the first frame update
    void Awake()
    {
        if (control == null)
            control = this;
    }

    public int CarCompleteLap()
    {
        return moneyToCompleteLap;
    }

    public Vector3 GetCheckpointPosition(int position)
    {
        return checkpointsPositions[position].position;
    }

    public CarManager HasOwner(int position)
    {
        //return checkpointsPositions[position].GetComponent<Checkpoint>().carOwner;
        return null;
    }
}
