using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {
		public enum rotationAxis {mouseXandY=0,mouseX=1,mouseZ=2}
		public rotationAxis axiS= rotationAxis.mouseXandY;
		float speed=0.5f;
		float sensitiveX = 15F;
		float sensitiveY = 15F;
//		float minX = -360F;
//		float maxX = 360F;
		float minY = -60F;
		float maxY = 60F;

		float rotY = 0F;

		public GameObject genObj;
		private CenterGen centerGen;
		
	// Use this for initialization
	void Start () {
		centerGen = genObj.GetComponent("CenterGen") as CenterGen;
	
	}
	
	// Update is called once per frame
	void Update () {
		float transverse=Input.GetAxis("Horizontal")*speed;
		float forward=Input.GetAxis("Vertical")*speed;
		transform.Translate(transverse,0,forward);
		//eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
		if(axiS==rotationAxis.mouseXandY){
			float rotX =transform.localEulerAngles.y+Input.GetAxis("Mouse X")*sensitiveX;
			rotY += Input.GetAxis("Mouse Y")*sensitiveY;
			rotY = Mathf.Clamp(rotY,minY,maxY);
			transform.localEulerAngles=new Vector3(-rotY,rotX,0);
		}
	centerGen.DistFlipIt();
		Collider[] hitColliders = Physics.OverlapSphere(transform.position,20.0f);
		for(int i=0;i<hitColliders.Length;i++){
			hitColliders[i].SendMessage("LogPos");
			//DistFlipIt是变幻BlockConnect里面总体的distFlipFlopTotal
			//LogPos是变换各个TM对应的distFlipFlop，TM接到命令以后再回呼BC相应的Func变换对应的distFlipFlop
		}
	}


}
