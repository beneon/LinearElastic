using UnityEngine;
using System.Collections;

public class PeriGen : MonoBehaviour {
	private Vector3[,] vectorList;
	private Vector3[][] vectorListExt;
	private Vector3[] periVect;
	private int[] overlapCount;
	private float step;

	// Use this for initialization
	void Start () {
		vectorList = new Vector3[4,4];
		vectorListExt = new Vector3[4][];
		overlapCount = new int[25];
		for(int i=0;i<vectorListExt.Length;i++){
			vectorListExt[i] = new Vector3[25];
		}
		periVect = new Vector3[25];
		step=1.0f;
		CentralSet();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void CentralSet(){
	for(int i=0;i<4;i++){
	for(int j=0;j<4;j++){
		vectorList[j,i]=new Vector3(j*(step+j),(i-2)*(i-2),i*step);
		PeriSet(j,i,vectorList[j,i]);
	}}
	}
	void PeriSet(int x,int y, Vector3 refV){
		vectorListExt[0][x+5*y+6]=new Vector3(refV.x+step/2,refV.y+step/2,refV.z+step/2);
		vectorListExt[1][x+5*y+5]=new Vector3(refV.x-step/2,refV.y+step/2,refV.z+step/2);
		vectorListExt[2][x+5*y+1]=new Vector3(refV.x+step/2,refV.y+step/2,refV.z-step/2);
		vectorListExt[3][x+5*y]=new Vector3(refV.x-step/2,refV.y+step/2,refV.z-step/2);
		overlapCount[x+5*y+6]+=1;
		overlapCount[x+5*y+5]+=1;
		overlapCount[x+5*y+1]+=1;
		overlapCount[x+5*y]+=1;
	}
	Vector3 PeriAverage(int rank){
		Vector3 averageV=new Vector3(0,0,0);
		for(int i=0;i<4;i++){
			averageV=averageV+vectorListExt[i][rank]/overlapCount[rank];
		}
		Debug.Log("count is "+overlapCount[rank]+",the Vector is "+vectorListExt[0][rank]+","+vectorListExt[1][rank]+","+vectorListExt[2][rank]+","+vectorListExt[3][rank]+",average:"+averageV);
		return averageV;
	}
	void OnDrawGizmos(){
		for(int i=0;i<4;i++){
		for(int j=0;j<4;j++){
			Gizmos.color=Color.blue;
			Gizmos.DrawCube(vectorList[i,j],new Vector3(0.2f,0.2f,0.2f));
		}
		}
		for(int k=0;k<25;k++){
			Gizmos.color=Color.red;
			Gizmos.DrawCube(PeriAverage(k),new Vector3(0.2f,0.2f,0.2f));
		}
	}


}
