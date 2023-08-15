using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysicsScript2 : MonoBehaviour
{
    //下方SerializeField对Unity字段序列化，这意味着它已保存在Unity编译器中公开
    //该字段仍然不受该类之外的代码影响
    [SerializeField,Range(0f,100f)]
    float maxSpeed = 10f;//调整速度

    [SerializeField,Range(0f,100f)]
    float maxAcceleration = 10f,maxAirAcceleration = 1f;//调整地面加速度和空中加速度

    [SerializeField,Range(0f,10f)]
    float JumpHeight = 2f;//控制跳跃高度

    [SerializeField,Range(0,5f)]
    int maxAirJumps = 0;//控制空中跳跃段数（比如二段跳之类的）
    
    [SerializeField,Range(0,90)]
    float maxGroundAngle = 25f;//最大地面角度阈值

    float minGroundDotProduct;

    int JumpPhase;//跟踪跳跃段数

    bool desiredJump;//检测是否跳跃
    
    int groundContactCount;//计算出拥有多少个地面接触点

    bool OnGround => groundContactCount > 0;
    //该定义方法与
    //bool OnGround{
    // get{
    //   return groundContactCount>0;
    // }
    // }

    Rigidbody body;

    private void OnValidate() {//可以用来验证数据
        minGroundDotProduct = Mathf.Cos(maxGroundAngle*Mathf.Deg2Rad);//检查角度
        //用Mathf.Deg2Rad将度数转换为弧度
    }
    
    void Awake() {
        body =  GetComponent<Rigidbody>();//拿到刚体
        OnValidate();//在构建时计算
    }

    Vector3 velocity,desiredVelocity;//跟踪速度,实现获取和设置分离

    Vector3 contactNormal;//跟踪当前法线，实现小球斜坡沿法线跳跃
    void Update(){
        Vector2 playerInput;//用一个Vector2来存储玩家的二维移动指令
        playerInput.x = Input.GetAxis("Horizontal");//让小球水平移动
        playerInput.y = Input.GetAxis("Vertical");//让小球垂直移动
        desiredJump |= Input.GetButtonDown("Jump");//让小球起跳
        // 但是我们有可能最终不会调用下一帧的FixedUpdate，在这时，desiredJump要设置回false，desiredJump将被忽略。
        // 可以通过布尔“或”运算或“或”赋值将检查与其先前的值相结合来防止这种情况。
        //这样一来，它一经启用便保持为true，直到我们将其显式设置回false。

        //上面这种方法控制移动是一种默认绑定
        //你可以从Edit——Project Setting——input中查看相关的设置
        playerInput = Vector2.ClampMagnitude(playerInput,1f);
        // playerInput.Normalize(); //该函数确保矢量不大于1,或者是使用上方ClampMagnitude方法
        //确保矢量不大于1可以让斜着走和其它方向的移动保持相同速度

        desiredVelocity = new Vector3(playerInput.x,0f,playerInput.y)*maxSpeed;

        GetComponent<Renderer>().material.SetColor(//根据接触点的数量来进行计数
            "_Color",Color.white*(groundContactCount*0.25f)
        );
    }

    void FixedUpdate() {//以固定的时间步长调整步速，默认值为0.02，每秒调用50次
        UpdateState();//统一使用UpdateState方法更改状态
        AdjustVelocity();//一下代码全部整合到AdjustVelocity之中
        // //根据你的帧速率，FixedUpdate每次调用时候，Update可以被调用零次，一次或多次。
        // //每个帧都会发生一系列FixedUpdate调用，
        // //然后调用Update，然后渲染该帧d。
        // //当物理时间步长相对于帧时间太大时，这可以使物理仿真的离散性质变得明显。
        // velocity = body.velocity;//检索出速度
        // float acceleration = onGround ? maxAcceleration:maxAirAcceleration;
        // //↑如果在地面上使用地面加速度，如果在空中使用空气加速度
        // float maxSpeedChange = acceleration * Time.deltaTime;//更新速度的幅度
        // // if(velocity.x < desiredVelocity.x)
        // // {
        // //     velocity.x = Mathf.Min(velocity.x+maxSpeedChange,desiredVelocity.x);//选取最小值消除过冲
        // // }
        // // else if(velocity.x>desiredVelocity.x){
        // //     velocity.x = Mathf.Max(velocity.x-maxSpeedChange,desiredVelocity.x);//选取最大值消除过冲效应
        // // }
        // //上方判定代码的简化：
        // //该函数返回值为    1+3<2? 1+3:2
        // velocity.x = Mathf.MoveTowards(velocity.x,desiredVelocity.x,maxSpeedChange);
        // velocity.z = Mathf.MoveTowards(velocity.z,desiredVelocity.z,maxSpeedChange);
        
        if(desiredJump){//检查是否跳跃
            desiredJump = false;
            Jump();
        }
        // onGround = false;
        ClearState();
    }

    void UpdateState(){//更改状态函数
        velocity = body.velocity;//记录当前速度
        if(OnGround){
            JumpPhase = 0;
            if(groundContactCount > 1){
                contactNormal.Normalize();//改变当前向量让它归一化成为适当的法线向量
            }
        }
        else{
            contactNormal = Vector3.up;//当没有触碰地面时，空气跳跃方向仍然向上    
        }
    }

    void Jump(){//跳跃函数
        if(OnGround || JumpPhase<maxAirJumps){
            JumpPhase +=1;
            float jumpSpeed = Mathf.Sqrt(-2f*Physics.gravity.y*JumpHeight);//为了让向上速度不超过单次跳跃速度上限
            float alignedSpeed = Vector3.Dot(velocity,contactNormal);//检查与接触法线对齐的速度的方法，使用点积来找到该速度
            if(alignedSpeed>0f){
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed,0f);
                //如果已经有向上的速度，则在将其添加到速度的Y分量之前，将其从跳跃速度中减去，这样就不会超速
                //用max函数解决已经快于跳跃速度但不希望减速的情况
            }
            velocity += contactNormal*jumpSpeed;//按照接触法线的比例将速度缩放至当前速率上
            //通过控制速度让其达到应该具有的高度，高中公式 v=sqrt(-2gh)
        }
    }

    void ClearState(){//重置状态
        groundContactCount = 0;//将计数设置为0
        contactNormal = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision) {//Collision获得碰撞信息
        EvaluateCollision(collision);//将collision信息转交给EvaluateCollision处理
    }

    private void OnCollisionStay(Collision collision) {//Collision获得碰撞信息
        EvaluateCollision(collision);//将collision信息转交给EvaluateCollision处理
    }
    
    void EvaluateCollision(Collision collision){
        for(int i= 0;i<collision.contactCount;i++){//contactCount用于找到接触点数量
            Vector3 normal = collision.GetContact(i).normal;//通过GetContact方法遍历所有点，并访问法线属性
            // onGround |= normal.y>=minGroundDotProduct;//这里是通过接收法线方向来判定是碰到了地面还是墙体
            if(normal.y >= minGroundDotProduct){//接触到地面时存储当前法线
                groundContactCount +=1;
                contactNormal += normal;//积累法线，计算平均法线方向
            }
        }
        
    }

    Vector3 ProjectOnContactPlane(Vector3 vector){//寻找与平面对齐的向量
        return vector - contactNormal*Vector3.Dot(vector,contactNormal);//找到在平面上的投影
    }

    void AdjustVelocity(){//在斜坡上突然反转方向时，球体不再与地面失去接触
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        //通过在接触平面上投影右向量和向前向量来确定投影的X轴和Z轴
        //通过归一化以获得正确的方向

        float currentX = Vector3.Dot(velocity,xAxis);
        float currentZ = Vector3.Dot(velocity,zAxis);
        //将当前速度投影到两个向量上，以获得相对的X和Z速度

        float acceleration = OnGround? maxAcceleration:maxAirAcceleration;
        float maxSpeedChange = acceleration*Time.deltaTime;

        float newX = 
            Mathf.MoveTowards(currentX,desiredVelocity.x,maxSpeedChange);
        float newZ = 
            Mathf.MoveTowards(currentZ,desiredVelocity.z,maxSpeedChange);
        //↑计算新的X和Z速度，但是现在相对于地面

        velocity += xAxis*(newX-currentX)+zAxis*(newZ - currentZ);
        //通过沿相对轴添加新旧速度之间的差异来调整速度
    }

}