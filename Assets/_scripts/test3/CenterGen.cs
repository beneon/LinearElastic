﻿using UnityEngine;
using System.Collections;

public class CenterGen : MonoBehaviour {

	public Rigidbody targetObj;
	public Rigidbody [,,] voxelCenters;
	public Vector3[,,] voxelCentersPos;
	public byte[,,] voxelData;
	private bool[,,] distFlipFlop;//记录某个voxel是否在绘制范围内
	public float step;

	public struct V3_int
	{
		public int x,y,z;
		public V3_int(int p1,int p2,int p3){
			x=p1;
			y=p2;
			z=p3;
		}
	}
	public V3_int VM_test=new V3_int(5,5,5);//因为其他地方还需要引用这个V3_int，所以放在start之前
	//----------------------springforce related
	public Vector3 [,,,] springForces;
	public bool [,,,] springFlipFlop;
	public bool flipFlop;
	public float miu;


	// Use this for initialization
	void Start () {
		step=1.0f;
		miu=2.0f;
		MatrixInitiate(VM_test.x,VM_test.y,VM_test.z);
		for(int i=0;i<VM_test.x;i++){
		for(int j=0;j<VM_test.y;j++){
		for(int k=0;k<VM_test.z;k++){
			VoxelSet(i,j,k,step,1);
		}}}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void MatrixInitiate(int x,int y,int z){
		voxelCenters=new Rigidbody[x,y,z];
		voxelCentersPos = new Vector3[x,y,z];
		voxelData = new byte[x,y,z];
		distFlipFlop=new bool[x,y,z];
		//----------springforce related
		springForces = new Vector3[x,y,z,6];
		springFlipFlop = new bool[x,y,z,6];
	}

	public void VoxelSet(int x, int y, int z,float step,byte n){
		if (x < 0 || x >= VM_test.x || y < 0 || y >= VM_test.y || z < 0 || z >= VM_test.z) {
			Debug.Log("error,out of range");
		}else{
		voxelCentersPos[x,y,z]=new Vector3(x*step,y*step,z*step);
		voxelCenters[x,y,z]=Instantiate(targetObj,voxelCentersPos[x,y,z],Quaternion.identity) as Rigidbody;
		voxelCenters[x,y,z].constraints = RigidbodyConstraints.FreezeRotation;
		TargetMark tmTemp = voxelCenters[x,y,z].GetComponent("TargetMark") as TargetMark;
		tmTemp.x=x;
		tmTemp.y=y;
		tmTemp.z=z;
		tmTemp.genObj=gameObject;
		voxelData[x,y,z] = n;
		}
	}
	public byte VoxelGet(int x,int y,int z){
		if (x < 0 || x >= VM_test.x || y < 0 || y >= VM_test.y || z < 0 || z >= VM_test.z) {
			return 0;
		}else{
			return voxelData[x,y,z];
		}
	}
	public void DistFlipIt (){
		for(int i=0;i<VM_test.x;i++){
			for(int j=0;j<VM_test.y;j++){
				for(int k=0;k<VM_test.z;k++){
					distFlipFlop[i,j,k]=false;
		}}}
	}
	public void DistFlipThis(int x,int y,int z){
		distFlipFlop[x,y,z]=true;
	}
	public bool DistDisplay(int x,int y,int z){
		if(distFlipFlop[x,y,z]){
			return true;
		}else{
			return false;
		}
	}
	//--------------spring force related--------------------
	void FixedUpdate(){
		SpringForceUpdate(VM_test.x,VM_test.y,VM_test.z);
	}
	void SpringForceUpdate(int x, int y, int z){
		//x,y,z是整个matrix的大小
		flipFlop=!flipFlop;
		for(int i=0;i<x;i++){
			for(int j=0;j<y;j++){
				for(int k=0;k<z;k++){
					if(VoxelGet(i,j,k)==1){
					//6 conditions,只需要在当前Voxel有block的情况下才需要继续
					//新建6个方向的spring所相对的targetV
					V3_int sLeft=new V3_int(i-1,j,k);
					V3_int sRight=new V3_int(i+1,j,k);
					V3_int sDown = new V3_int(i,j-1,k);
					V3_int sUp = new V3_int(i,j+1,k);
					V3_int sBack = new V3_int(i,j,k-1);
					V3_int sFoward = new V3_int(i,j,k+1);
					SixSprings(i,j,k,sLeft,0,1);
					SixSprings(i,j,k,sRight,1,0);
					SixSprings(i,j,k,sDown,2,3);
					SixSprings(i,j,k,sUp,3,2);
					SixSprings(i,j,k,sBack,4,5);
					SixSprings(i,j,k,sFoward,5,4);
					voxelCentersPos[i,j,k]=voxelCenters[i,j,k].position;
					}
				}}}
	}
		void SixSprings(int x,int y,int z, V3_int targetV, int localC, int targetC){
		//这里做了几个判定，首先是当前点是否有移动，通过对比当前rigidbody位置和之前存储的位置
		//之后是对比周围6点是否有移动，同样是前后比较
		//tm1获取，之后设置里面的一个bool，表示这个tm现在动了。
		bool positionChanged = voxelCenters[x,y,z].position != voxelCentersPos[x,y,z];
		if(VoxelGet(targetV.x,targetV.y,targetV.z)==1){
		bool targetPosChanged = voxelCenters[targetV.x,targetV.y,targetV.z].position != voxelCentersPos[targetV.x,targetV.y,targetV.z];
		TargetMark tm1 = voxelCenters[x,y,z].GetComponent("TargetMark") as TargetMark;	
		if(positionChanged || targetPosChanged){
		tm1.moving = true;
			if(springFlipFlop[targetV.x,targetV.y,targetV.z,targetC]==flipFlop){
			//两个点共享一个spring，所以如果对面的点已经是更新过的话那我们这边就不必要重新算了，直接反向就好
				springForces[x,y,z,localC]=-springForces[targetV.x,targetV.y,targetV.z,targetC];
			}else{
			//如果对面的点也没算过当前这条spring，那就要重新计算，首先把方向算出来（正反倒不一定）
				springForces[x,y,z,localC]=voxelCenters[targetV.x,targetV.y,targetV.z].position-voxelCenters[x,y,z].position;
			//然后具体的数值就代入linearElastic来算
				springForces[x,y,z,localC]=LinearElastic(miu,step,springForces[x,y,z,localC]);
			}
			//if(VoxelGet(x+1,y,z)*VoxelGet(x-1,y,z)*VoxelGet(x,y+1,z)*VoxelGet(x,y-1,z)*VoxelGet(x,y,z-1)*VoxelGet(x,y,z+1)!=0){
			//上面这个判断式的意思是固定matrix周围6个面上面的所有的点。下面先只固定底面
			if(VoxelGet(x,y-1,z)!=0){
			if(VoxelGet(x-1,y,z)==0 || VoxelGet(x+1,y,z)==0){
				voxelCenters[x,y,z].constraints = RigidbodyConstraints.FreezePositionX;
			}
			if(VoxelGet(x,y,z-1)==0 || VoxelGet(x,y,z+1)==0){
				voxelCenters[x,y,z].constraints = RigidbodyConstraints.FreezePositionZ;
			}
			voxelCenters[x,y,z].AddForce(springForces[x,y,z,localC]);
			}
			springFlipFlop[x,y,z,localC]=!springFlipFlop[x,y,z,localC];
			//不管是通过哪种方式变换,完成后都需要把相应springforce的flipflop变换一下，可以！也可以=flipflop，应该是等效的
		}else{
		tm1.moving=false;
		}
		}
	}
	//下面这个是把elastic force里面计算的公用部分脱离出来
	//linear elastic can be calulated with miu*diff_in_len*vect_norm
	//let miu be 1, diff_in_len be (target.position-centerPoint.position)-len_ori, vect_norm be (target.pos-centerPoint.pos).normalized
	//let len_ori be 1( the initial dist between neighbour p to central p)
	//上面的算法里面，如果对diff_in_len取绝对值的话力的方向会出错

	//虽然前面sixspring已经独立出来了，不过为了少写一些springforce[,,,]还是保留这一个func吧
	Vector3 LinearElastic(float miu,float len_ori, Vector3 forceV){
		float diff_in_len = forceV.magnitude - len_ori;
		forceV=forceV.normalized*miu*diff_in_len;
		return forceV;
	}
	//----------------------------------
}



