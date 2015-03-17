using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugTest2 : MonoBehaviour {

	public Vector3[] recpVect;
	public StructTest test1;
	// Use this for initialization
	void Start () {
		test1 = gameObject.GetComponent("StructTest") as StructTest;
		Debug.Log(test1.srcVect.Length);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
