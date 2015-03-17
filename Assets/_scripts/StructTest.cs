using UnityEngine;
using System.Collections;

public class StructTest : MonoBehaviour {

	public Vector3[] srcVect;
	// Use this for initialization
	void Start () {
		srcVect = new Vector3[5];
		for(int i=0;i<5;i++){
			srcVect[i]=new Vector3(i,i,i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
