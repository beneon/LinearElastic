using UnityEngine;
using System.Collections;
public class BlockConnect : MonoBehaviour {

	public Rigidbody targetObj;//这个prefab的插槽暂时留着2-27 13:27
	private Rigidbody[,,] voxelCenters;
	private Vector3[,,] voxelCentersPos;
	public byte[,,] voxelData;
	public Vector3 [,,,] springForces;
	public bool [,,,] springFlipFlop;
	public bool flipFlop;
	public struct V3_int
	{
		//其实本来这个V3_int就和Vector3差不多，不过vector3用的是float，但是我们只用int就可以了，所以自己建一个struct
		//这个struct目前主要用在设定整个matrix的尺寸
		public int x,y,z;

		public V3_int(int p1, int p2, int p3){
			x=p1;
			y=p2;
			z=p3;
		}
	}
	private V3_int VM1;
	//上面两行代码基本和之前test1一样，不过这次需要做6个目标，所以用了Transform[]这个数组形式
	//**但是数组在用以前需要新建一下voxelCenters = new Transform[]
	
	void Start () {
		VM1=new V3_int (6, 6, 6);//前面不要再添加 VM_size了
		VoxelMatrixModel(VM1.x,VM1.y,VM1.z);
		for(int i=0;i<VM1.x;i++){
			for(int j=0;j<VM1.y;j++){
				for(int k=0;k<VM1.z;k++){
					VMSet(i,j,k,1,1);
					//i,j,k:position;1:step is 1;1:n is 1
		}}}
		
	}
	
	void Update () {
	
	}

	void FixedUpdate(){
		SpringForceUpdate(VM1.x,VM1.y,VM1.z);
	}


	void VoxelMatrixModel(int x,int y,int z){
		//这个是建立一个空白的一定尺寸体素点阵的函数
		//需要先在外面声明一个voxelData才行
		//byte这个类型可以储存0~255之间的数值
		//同时现在把voxelCenters也放在这里新建
		//这里是建立【容量】，不是内容，但是在start里面VMSet也是用的x>y>z循环，所以其实也是不大OK的。
		voxelData = new byte[x,y,z];
		voxelCenters = new Rigidbody[x,y,z];
		voxelCentersPos = new Vector3[x,y,z];
		springForces = new Vector3[x,y,z,6];
		springFlipFlop = new bool[x,y,z,6];
	}
	void SpringForceUpdate(int x, int y, int z){
		//x,y,z是整个matrix的大小
		flipFlop=!flipFlop;
		for(int i=0;i<x;i++){
			for(int j=0;j<y;j++){
				for(int k=0;k<z;k++){
					if(VMGet(i,j,k)==1){
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
	
	//这个是根据对应rigidbody和当前rigidbody计算springforce的func
	void SixSprings(int x,int y,int z, V3_int targetV, int localC, int targetC){
		float miu = 1.0f;
		float len_ori = 1.0f;
		bool positionChanged = voxelCenters[x,y,z].position != voxelCentersPos[x,y,z];
		if(VMGet(targetV.x,targetV.y,targetV.z)==1){
		bool targetPosChanged = voxelCenters[targetV.x,targetV.y,targetV.z].position != voxelCentersPos[targetV.x,targetV.y,targetV.z];
		TargetMark tm1 = voxelCenters[x,y,z].GetComponent("TargetMark") as TargetMark;
		if(positionChanged || targetPosChanged){
			if(springFlipFlop[targetV.x,targetV.y,targetV.z,targetC]==flipFlop){
			tm1.showCube = true;
			//两个点共享一个spring，所以如果对面的点已经是更新过的话那我们这边就不必要重新算了，直接反向就好
				springForces[x,y,z,localC]=-springForces[targetV.x,targetV.y,targetV.z,targetC];
			}else{
			//如果对面的点也没算过当前这条spring，那就要重新计算，首先把方向算出来（正反倒不一定）
				springForces[x,y,z,localC]=voxelCenters[targetV.x,targetV.y,targetV.z].position-voxelCenters[x,y,z].position;
			//然后具体的数值就代入linearElastic来算
				springForces[x,y,z,localC]=LinearElastic(miu,len_ori,springForces[x,y,z,localC]);
			}
			if(VMGet(x+1,y,z)*VMGet(x-1,y,z)*VMGet(x,y+1,z)*VMGet(x,y-1,z)*VMGet(x,y,z-1)*VMGet(x,y,z+1)!=0){
			//这个判断式的意思是固定matrix周围6个面上面的所有的点。
			voxelCenters[x,y,z].AddForce(springForces[x,y,z,localC]);
			}
			//判断归判断，但是springforce算完了以后还是一定要把flipflop翻一下的。
			springFlipFlop[x,y,z,localC]=!springFlipFlop[x,y,z,localC];
			//不管是通过哪种方式变换,完成后都需要把相应springforce的flipflop变换一下，可以！也可以=flipflop，应该是等效的
		}else{
			tm1.showCube=false;
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





	
	byte VMGet(int x,int y,int z){
		if (x < 0 || x >= VM1.x || y < 0 || y >= VM1.y || z < 0 || z >= VM1.z) {
			return 0;
		} else {
			return voxelData [x, y, z];
		}
	}
	
	void VMSet(int x,int y,int z,int step,byte n){
		if( x < 0 || x >= VM1.x || y < 0 || y >= VM1.y || z < 0 || z >= VM1.z ){
			Debug.Log("error,out of range for VMSet");
		}else{
		voxelData[x,y,z]=n;
		//n=1的时候需要添加rigidbody
		//另外新的rigidbody出现的位置在相应的xyz坐标乘以step
		if (n == 1) {
			
			voxelCentersPos[x,y,z]=new Vector3(x*step,y*step,z*step);
			voxelCenters [x,y,z] = Instantiate (targetObj, voxelCentersPos[x,y,z],Quaternion.identity) as Rigidbody;
		}
		}
	}
}

	//周围点的生成可能还是放在VoxelMatrixModel比较好,所以下面这个neighbourGen就废用了吧
	//对于每一个点，周围均有6点
	//通过获取centerPoint 的transform然后在这个点周围六点instantiate
	//因为需要知道周围6点是不是已经生成了，所以要有一个点阵情况的数据，这个就是model 的工作了吧
	//我们需要一个4*4的点阵，但是还要需要周围边界，左右加1就是6*6的点阵
	//	void NeighbourGen(Transform centerPoint){
	//	for(int i=0;i<6;i++){
	//这里应该添加一个评估周围6点是否需要instantiate的判定
	//由于评估需要根据6点位置有所不同，所以停用for循环，改用分别设定
	//直觉上感觉应该在voxelmatrix里面添加一下关于target这边transform的编号
	//或者应该用object来写？但是还不会啊
	//		voxelCenters[i]=Instantiate(targetObj,centerPoint.position,centerPoint.rotation) as Transform;
	//	}
	//	
	//	}
