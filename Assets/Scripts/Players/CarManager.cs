using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarManager : MonoBehaviour
{
    public enum CarID
    {
        Car1,
        Car2
    }

    //public CarID carID = CarID.Car1;
    public GameObject Car;
    public int money = 1000;
    public bool[] checkPoints = new bool[16];
    //public CarControllerBase carController;

    //public TextMeshProUGUI moneyTXT;
    //public TextMeshProUGUI costCasillaTXT;

    // Start is called before the first frame update
    void Awake()
    {
        //carController = GetComponentInParent<CarControllerBase>();
        //carController.carManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -1)
        {
            //Respawn(true);
        }

        //moneyTXT.text = money.ToString();

        if(money < 0)
        {
            //LevelManager.control.NotifyToLose(carID);
        }
    }

    public void ReceiveID(int ID)
    {
        //Debug.Log("ID: " + ID);
        if (ID == 0)
        {
            checkPoints[ID] = true;
            return;
        }
        Debug.Log(checkPoints[ID - 1]);
        checkPoints[ID] = checkPoints[ID - 1];
        CheckFinishLap();
        
    }

    public void CheckFinishLap()
    {
        bool fullLap = true;

        for (int i = 0; i < checkPoints.Length; i++)
        {
            fullLap = fullLap && checkPoints[i];
        }

        if (fullLap)
        {
            money += CheckPointManager.control.CarCompleteLap();
            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i] = false;
            }
        }
    }

    public void Respawn(bool restMoney)
    {
        Vector3 positionToRespawn = new Vector3();
        CarManager isOwner = new CarManager();
        for (int i = 0; i < checkPoints.Length; i++)
        {
            if (!checkPoints[i])
                if (i == 0)
                {
                    positionToRespawn = CheckPointManager.control.GetCheckpointPosition(checkPoints.Length - 1);
                    //isOwner = CheckPointManager.control.HasOwner(checkPoints.Length - 1);
                    break;
                }
                else
                {
                    positionToRespawn = CheckPointManager.control.GetCheckpointPosition(i - 1);
                    //isOwner = CheckPointManager.control.HasOwner(i - 1);
                    break;
                }
        }

        if(restMoney)
        {
            if (isOwner)
            {
                isOwner.money += 200;
            }

            money -= 200;
        }

        //carController.StopCar();
        Debug.Log(positionToRespawn);
        Car.transform.position = positionToRespawn + new Vector3(0, 1, 0);
        Car.transform.rotation = Quaternion.Euler(Mathf.Abs(Car.transform.rotation.x), Car.transform.rotation.y, Mathf.Abs(Car.transform.rotation.z));
    }

    //public void CanBuy(Checkpoint trampa)
    //{
    //    switch (carID)
    //    {
    //        case CarID.Car1:
    //            if (Input.GetKey(KeyCode.Space))
    //            {
    //                trampa.BuyTramp(this);
    //            }
    //            break;
    //        case CarID.Car2:
    //            if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
    //            {
    //                trampa.BuyTramp(this);
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}
}
