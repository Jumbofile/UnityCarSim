using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}

public class CarController : MonoBehaviour {
    //Vars
	public List<AxleInfo> axleInfos; 
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public float maxBraking;
	public Rigidbody car;
	public Text speedo;
    public bool openDiff;
    public bool solidDiff;
    public bool slipDiff;
    public float antiRoll = 5000f;

	// finds the corresponding visual wheel
	// correctly applies the transform
	public void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) {
			return;
		}

		Transform visualWheel = collider.transform.GetChild(0);

		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);

        //Sets position of the wheel
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

	public void FixedUpdate()
	{
		float mph = car.velocity.magnitude * 2.237f;
		float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float braking = maxBraking;

		//Speedo
		speedo.text = mph.ToString("f0");
     
        //Only one diff can be selected at once
        if(openDiff == true)
        {
            slipDiff = false;
            solidDiff = false;
        }
        else if(slipDiff == true)
        {
            solidDiff = false;
            openDiff = false;
        }
        else
        {
            openDiff = false;
            slipDiff = false;
        }

        //AntiRoll
        antiRollBar();

        //Turning
        foreach (AxleInfo axleInfo in axleInfos) {
            //Braking(NEEDS A REVAMP)
            if (Input.GetKey (KeyCode.Space)) {
				axleInfo.leftWheel.motorTorque = 0;
				axleInfo.rightWheel.motorTorque = 0;
				axleInfo.leftWheel.brakeTorque = braking;
				axleInfo.rightWheel.brakeTorque = braking;
			} else {
				axleInfo.leftWheel.brakeTorque = 0;
				axleInfo.rightWheel.brakeTorque = 0;
			}

            //Steering
			if (axleInfo.steering) {
				if (Input.GetKey (KeyCode.A)) {
                    //Left
					axleInfo.leftWheel.steerAngle = steering * 1.4f; //Needs to be redone
					axleInfo.rightWheel.steerAngle = steering;
				} else if (Input.GetKey (KeyCode.D)) {
                    //Right
					axleInfo.leftWheel.steerAngle = steering;
					axleInfo.rightWheel.steerAngle = steering * 1.4f; // Needs to be redone
				} else {
                    //Straight
					axleInfo.leftWheel.steerAngle = steering;
					axleInfo.rightWheel.steerAngle = steering;
				}
			}
            //Power to wheels
			if (axleInfo.motor) {
                //Car wants to be moved
				if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)){
                    //Open differential, one wheel has power in turn
                    if (openDiff == true)
                    {
                        if (Input.GetKey(KeyCode.A))
                        {
                            //axleInfo.leftWheel.motorTorque = motor;
                            axleInfo.rightWheel.motorTorque = motor;
                        }
                        else if (Input.GetKey(KeyCode.D))
                        {
                            axleInfo.leftWheel.motorTorque = motor;
                            //axleInfo.rightWheel.motorTorque = motor;
                        }
                        else
                        {
                            axleInfo.leftWheel.motorTorque = motor;
                            axleInfo.rightWheel.motorTorque = motor;
                        }

                     //Solid differential, both wheels have the same amount of power in a turn
                    }
                    else if (solidDiff == true)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                    //Limited slip differential, variable wheel spinning
                    else if (slipDiff == true)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                //Car doesnt want to move
				} else {
					axleInfo.leftWheel.motorTorque = 0;
					axleInfo.rightWheel.motorTorque = 0;
				}

			}

            //Make the graphic wheels move
			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}
	}

    //Anti Rollbar (Based on Edys JavaScript version)
    public void antiRollBar()
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            bool groundedL = axleInfo.leftWheel.GetGroundHit(out hit);
            if (groundedL)
            {
                travelL = (-axleInfo.leftWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheel.radius) / axleInfo.leftWheel.suspensionDistance;
            }

            bool groundedR = axleInfo.rightWheel.GetGroundHit(out hit);
            if (groundedR)
            {
                travelR = (-axleInfo.rightWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.rightWheel.radius) / axleInfo.rightWheel.suspensionDistance;
            }

            float antiRollForce = (travelL - travelR) * antiRoll;

            if (groundedL)
            {
                GetComponent<Rigidbody>().AddForceAtPosition(axleInfo.leftWheel.transform.up * -antiRollForce,
                    axleInfo.leftWheel.transform.position);
            }

            if (groundedR)
            {
                GetComponent<Rigidbody>().AddForceAtPosition(axleInfo.rightWheel.transform.up * antiRollForce,
                    axleInfo.rightWheel.transform.position);
            }
        }
    }
}