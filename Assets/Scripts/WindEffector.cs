using UnityEngine;
using System.Collections;

public class WindEffector : MonoBehaviour {

    WindGenerator WindGenerator;
    Rigidbody Rigidbody;

    void Start () {
        WindGenerator = FindObjectOfType<WindGenerator>();
        Rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update () {
        Rigidbody.AddForce(WindGenerator.GenerateImpulseForce(), ForceMode.Impulse);
    }

}
