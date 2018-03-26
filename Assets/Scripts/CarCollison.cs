using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollison : MonoBehaviour {
    public Rigidbody car;
    public GameObject wheels;
    public GameObject phys1;
    public GameObject phys2;
    public GameObject phys3;
    public GameObject phys4;
    public GameObject explo;
    float speed;
    bool exlp = false;
    float rand;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		speed = car.velocity.magnitude * 2.237f;
        rand = Random.Range(-10f, 10f);          
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("TRUE");

        //When rigidbody collides with wall
        if (collision.gameObject.tag == "wall" && speed > 90 && exlp == false)
        {
            exlp = true;
            car.AddForce(5 * rand, speed * 100, 0, ForceMode.Impulse);
            print("TRUE");
            if (car.rotation.z != 0)
            {
                //Fixedcamera lol
            }
            wheels.SetActive(false);
            //Set to array
            //Activates the physics wheels and breaks them off the car
            phys1.SetActive(true);
            phys1.GetComponent<Rigidbody>().AddForce(0, (rand * speed), 1, ForceMode.Impulse);
            phys4.SetActive(true);
            phys4.GetComponent<Rigidbody>().AddForce(0, (rand * speed), 1, ForceMode.Impulse);
            phys3.SetActive(true);
            phys3.GetComponent<Rigidbody>().AddForce(0, (rand * speed), 1, ForceMode.Impulse);
            phys2.SetActive(true);
            phys2.GetComponent<Rigidbody>().AddForce(0, (rand * speed), 1, ForceMode.Impulse);
            explo.SetActive(true);

        } 
    }
}
