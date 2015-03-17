using UnityEngine;
using System.Collections;
//这个代码主要是为了练习怎么进行debug绘图，方便后面进一步操作
//gizmos绘图除了画线以外还有很多其他的功能
//注意Vector3.zero和 new Vector3(1,0,0)两个地方
//Vector3本身是一个structure，zero是他的一个static var
//new Vector3是利用Vector3这个constructor建一个新的Vector3 结构
//2-26 1:18
//接下来要复习一下Debug.DrawLine的用法，同时复习一下获取场景obj位置的方法：画一个从0点到场景中某物体的线，这个物体还要在运行中拖动一下位置
//debug的线绘制暂时就这么多
//2-26 17:10

public class DrawLineTest : MonoBehaviour {

	//为当前脚本指定一个目标的办法，有几个
	//1 public Transform transformOfEmpty;这种方法是最直接的，声明一个public变量，然后把一个
	//GameObj拖动到上面<prefab恐怕没法这么干，但是还有后面的办法
	//2 GameObject.FindGameObjectsWithTag<这种方法是通过object上面的tag来搜相应的Gameobj
	//但是也不适合找prefab
	//3 在prefab里面public GameObject worldGO;
	//然后在自己这个script里面将worldGo设置成自己，也就是gameObject
	//4 声明一个public 的object，然后把prefab拖上来，instantiate,这里用的是第4种
	//instantiate的是一个object，所以type要统一
	//从public的槽位，private的声明，到instantiate的as都要统一

	public Transform targetObj;
	private Transform targetTemp;
	void Start () {
		targetTemp = Instantiate(targetObj,transform.position,transform.rotation) as Transform;
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine(transform.position,targetTemp.position);

	}
	//targetTemp需要在start()外面进行声明，否则在update里面没办法用。
	void OnDrawGizmos(){
		Gizmos.DrawLine(Vector3.zero,new Vector3(1,0,0));
		Gizmos.DrawLine(Vector3.zero,new Vector3(-1,0,0));
		Gizmos.color=Color.yellow;
		Gizmos.DrawLine(Vector3.zero,new Vector3(0,1,0));
		Gizmos.DrawLine(Vector3.zero,new Vector3(0,-1,0));
	}
	void OnDrawGizmosSelected(){
		Gizmos.color=Color.red;
		Gizmos.DrawLine(Vector3.zero,new Vector3(0,0,1));
		Gizmos.DrawLine(Vector3.zero,new Vector3(0,0,-1));
	}
	//Gizmos很适合用来debug，但是只能用在OnDrawGizmos和OnDrawGizmosSelected上面
	//如果当前这个script放在一个empty object上面，使用OnDrawGizmos的时候不管obj是否选中都会绘画
	//OnDrawGizmosSelected就要选中obj的时候才会运行
}
