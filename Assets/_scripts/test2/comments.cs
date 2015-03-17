using UnityEngine;
using System.Collections;

public class comments : MonoBehaviour {
}
//On BlockConnect-------------------------------------------------------------------
//这个代码在之前绘制线的基础上将中心点和周围六个点相连，然后对中心点进行一个往复运动
//六个点的定位首先initiate 6个transform，之后以当前位置为基准，分别向6个轴向移动<<6个点是将空间划分
//<<为voxel以后得来的
//之后debug.drawline来画演示线
//接下来要在x轴方向做一下linear elastic的测试--2-25,20:40---------------------------
//对中心点的操作和线性弹性的定义在另外一个module里面
//centerMovement是假定周围6点固定，计算受力
//同时还要加上阻尼
//discretised Lagrange equation: M*a+v*damping+linear_elastic=external_force
//M*a would be the AddForce vector, external_force is still zero now.
//damping was set with .drag
//接下来需要在6个点的周围各建立6个点，然后移动中心点的时候做到周围6点跟着移动。
//首先是建立6个点的过程要独立出来:NeighbourGen(Transform centerPoint)
//2-27,10:29--------------------------------
//建立了3个voxelMatrix相关的函数，一个是生成的，一个是获取相应点byte的，一个是写入相应点byte的
//虽然目前都只有一行的代码，但是单独成一个function后期加东西好加。而且总觉得应该将这个做成一个obj
//在添加新的功能以后之前的很多代码都不能用了，要小心修改
//2-27,22:41----------------------------
//目前保留的有DebugConnect:绘制辅助线，CenterMovement：控制中心点的移动，neighbourGen的功能已经并入VoxelMatrixModel，而instantiate也要交给VMSet来进行比较好
//[声明的位置]，如果VM_size VM1放在了function外面，那么各个function都可以应用，但是如果放在了里面就不行了。||即便在外面声明过，但是如果在function里面重新声明的话，
//那么function里面对VM1做的操作都是针对function内部的，对外部声明的VM1没有影响<<目前对VMGet的调试基本ok，还试着使用了一下struct
//2-28 11:35 现在准备把和rigidbody受力有关的内容全部搬到TargetMark.cs里面去。在生成TM这些rigidbody的同时就顺便把他的位置传过去（用PositionRef）
//觉得应该把Springforce单独保存起来，因为对于相邻的两个点，一个spring对他们的力大小是一样的，方向不同而已，重复计算不划算。
//现在是在这里根据rigidbody们的位置进行统一计算，保存结果在一个4维Vector3数组里面[x,y,z,6]
//2-28 15:47
//计算过程中根据整体flipflop和自身flipflop比较决定是否更新，更新时对照相邻点对应SF看是否能直接使用
//对应方法：比如[x,y,z,5]<>[x,y,z+1,4]，左0，右1，下2，上3，后4，前5
//3-1 0:16
//bool类型前面加一个!就是否运算，Vector3前面加一个-减号就是反向
//3-1 14:24
//实现了弹性运动，现在给一点以初始力以后可以看看这个点和周围点是怎么运动的，还蛮有意思的
//三层循环用的ijk，结果里面却还是用xyz，所以一开始动不起来。
//现在springforce的运算，给rigidbody加力都在blockconnect这个脚本里面进行。targetmark里面现在只有gizmo标注的一点点程序。
//现在发现自建的V3_int也有点用处，原有的那个Vector3存储的是float，如果直接拿来用还得做一次floortoint
//现在直接自建一个存储int的三维数组就好了。springForce6个方向的力都用一个相同的func计算，通过改变参数实现6个方向的适应。
//但是目前全范围扫描也确实很没有效率哦:neutroPos是x,y,z * step,和neutroPos相比没变就不用算了。
//但是一旦这样子检测的话就会出错……<<出错的原因就是我把前后位置相等写成了changed...很多时候bug都是因为脑残出来的，没那么化学的。好好检查，一点一点排查一般都能找出来！
//好了，现在elastic 这一块已经ok了，尽量不要再对其进行修改，接下来进行下一个模块。
//ps,现在的显示效果配合电音好帅
//todo：目前linear elastic和外界的接口还没有想好
//todo:注意VMSet初始的时候把全部容量都生成voxelcenters了，这个到后期需要根据模型实际情况进行调整。
//接下来需要重新另外起一个程序看看怎么进行collision detect。而elastic这一块目前暂时为止。

//On BlockConnect-------------------------------------------------------------------end
//
//On CollisonDetect-----------------------------------------------------------------------
//要进行Collision，首先要把voxelCenter转换成mesh；
//On MeshGen----------------------------------------------------------------------------------
//Meshgen之前先把cameraMovement给弄清楚先
//On CameraMove--------------------------------------------------------------
//cameraMove的script当然要挂在camera上
//左右前后都好办,x=Input.GetAxis("Horizontal")*speed,z=Input.GetAxis("Vertical")*speed,tranform.Translate(x,0,z);如果想上下移动再加一个轴就可以
//旋转比较麻烦。evernote里面有一个示例；
//里面首先用到了一个[enum]的类型，感觉不错，其实可以在设置voxel内容的时候使用
//     public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
//     public RotationAxes axes = RotationAxes.MouseXAndY;
//On OverlapSphere------------------------------------------------------------
//这里我希望能引入drawing distance的概念
//physics里面有一个overlapSphere的工具，所以还是用rigidbody+collider吧，这样子比较好
//另外，如果一个面是背对着我们的（法向和摄像机方向一样？）那么这个面肯定就不用化了
//OverlapSphere这个工具现在也是联合flipflop这种bool联合使用。但是要注意对于总体的flip要先于各个子obj的flip，否则就会不能正常运行。更加不可以把总体的flip放在循环里面。
//对于obj之间，尽量不要直接设置变量，尽量通过func交谈
//最早的flipflop设计是把cube显示与否也放在LogPos里面，但是这样的话如果范围移开以后，logpos不再启动，那么这些点就一直留着了，更糟糕的是等下移回来的话可能还回不来（各自的flip和整体的flip错开了）
//所以现在是首先distFlipit，这个是把所有的各自flip设置成false，然后在logpos里面叫distFlipthis，把范围内的flip成true，之后在tm的update里面按照各自的flip设置cubeshow
//之所以这么绕而不是直接在tm里面的update里设置成showcube=false然后logpos设置true就是因为update总是最优先的，如果在这里设置成false的话那你怎么也弄不回来了
//另外，不知道为什么，physics.Overlapsphere放在fixedupdate里面就会出错哦……导致tm对bc的引用无效
//On MeshGen------------------------------------------------------------
//过去meshgen只用考虑6个面，但是由于现在中心点可以变形，所以面的方向也不再固定6个方向
//那么怎么才能保证面是朝外的呢？不过面就算在怎么变形应该也不至于转90°，所以应该还好。
//现在的问题是，怎么样在已知中心点的情况下，绘制出包绕这些中心点的mesh？暂时不考虑平滑的问题
//应该是从中心点向外侧移动半个step，这样就出现了6个面上的中心点了
//然后根据这些6个面上面的点计算8个顶点：双线性插值：先横向上取两个中心点，然后取这两个中心点的中心点<实际中是直接对周围的中心点进行平均运算
//如果当前block是在边界上面的，那么最外层的点就是中心点向外沿平面移动
//定好点以后就开始画mesh
//--------------------meshgen-periVert GEN--------------------
//在Debug里面测试了一下，找到了几个关键点
//1.周围点用一个交错数组来实现vertExt[一个点最大重叠次数，一般是4][全部周围点总共的个数]
//	交错数组声明以后：
//		vectorListExt = new Vector3[4][];
//		for(int i=0;i<vectorListExt.Length;i++){
//			vectorListExt[i] = new Vector3[25];
//		}
//2.而对于一个周围点，他最多从四个周围的中心点进行平均，中心点周围4点从左下顺时针分别为n*y+x,n*(y+1)+x,n*(y+1)+x+1,n*y+x+1,其中n是一行里面周围点的数目，假定一行4个中心点，5个周围点，那么给定一个中心点的横向，纵向数量及vector3以后
//	void PeriSet(int x,int y, Vector3 refV){
//		vectorListExt[0][x+5*y+6]=new Vector3(refV.x+step/2,refV.y+step/2,refV.z+step/2);
//		vectorListExt[1][x+5*y+5]=new Vector3(refV.x-step/2,refV.y+step/2,refV.z+step/2);
//		vectorListExt[2][x+5*y+1]=new Vector3(refV.x+step/2,refV.y+step/2,refV.z-step/2);
//		vectorListExt[3][x+5*y]=new Vector3(refV.x-step/2,refV.y+step/2,refV.z-step/2);
//		overlapCount[x+5*y+6]+=1;
//		overlapCount[x+5*y+5]+=1;
//		overlapCount[x+5*y+1]+=1;
//		overlapCount[x+5*y]+=1;
//	}
//	这里overlapCount的目的是计数一个周围点从多少个中心点取值,然后通过,
//	Vector3 averageV=new Vector3(0,0,0);
//		for(int i=0;i<4;i++){
//			averageV=averageV+vectorListExt[i][rank]/overlapCount[rank];
//		}
//	这个来进行平均。
//	注意周围点的最终是一个一维数组，或者其实应该用二维数组？

