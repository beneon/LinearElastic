using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGen : MonoBehaviour {
	public List<Vector3> newVertices = new List<Vector3>();
	public List<int> newTriangles = new List<int>();
	public List<Vector2> newUV = new List<Vector2>();
	private CenterGen centerGen;//这个根据相应代码名字的不同还要更改哦，呃……不过好在下面只用改初始那一行就ok了
	private int vmX,vmY,vmZ;


	// Use this for initialization
	void Start () {
		centerGen = gameObject.GetComponent("CenterGen") as CenterGen;
		vmX=centerGen.VM_test.x;
		vmY=centerGen.VM_test.y;
		vmZ=centerGen.VM_test.z;
	}
	
	// Update is called once per frame
	void Update () {
	}

}
