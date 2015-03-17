//这个是各个rigidbody自己的script
using UnityEngine;
using System.Collections;

public class TargetMark : MonoBehaviour {

	public int x,y,z;//在这里提供记录自己属于哪个位置的插槽
	public bool showCube = false;
	public bool moving = false;
	public GameObject genObj;
	private CenterGen centerGen;
	// Use this for initialization
	void Start () {
		centerGen = genObj.GetComponent("CenterGen") as CenterGen;
	}
	
	// Update is called once per frame
	void Update () {
	if(centerGen.DistDisplay(x,y,z)){
			showCube=true;
		}else{
			showCube=false;
		}
	}

	void OnDrawGizmos(){
		if(moving){
			Gizmos.color=Color.blue;
		}else{
			Gizmos.color=Color.white;
		}
		if(showCube){
		Gizmos.DrawCube(transform.position, new Vector3(0.2f,0.2f,0.2f));
		}
	}
	void LogPos(){
		centerGen.DistFlipThis(x,y,z);
		
	}
}
