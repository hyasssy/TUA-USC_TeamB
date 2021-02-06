using UnityEngine;

public class ControllerSample : MonoBehaviour//DebugCamera用。カメラobjに直付け
{
    KeyCode[] movekeys = {KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.E};
    KeyCode[] rotatekeys = {KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow};
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float moveAcceleration = 2f;
    [SerializeField] float rotateAcceleration = 20f;
    float[] tempSpeed = {0,0,0};//{forward, horizontal, vertical}
    float[] tempRotateSpeed = {0,0};//{vertical, horizontal}
    public bool isAcceleratedMove, isAcceleratedRotate;
    

    private void Update() {
        if(isAcceleratedMove){
            AcceleratedMove();
        }else{
            Move();
        }
        if(isAcceleratedRotate){
            AcceleratedRotate();
        }else{
            Rotate();
        }
    }

    void Move(){
        int[] moveInputs = {0,0,0,0,0,0};
        for(int i=0;i<6;i++){
            if(Input.GetKey(movekeys[i])){
                moveInputs[i]++;
            }
        }

        Vector3 horizontalDis = Vector3.Scale(transform.forward, new Vector3(1,0,1)).normalized 
        * (moveInputs[0]-moveInputs[2]) * moveSpeed * Time.deltaTime + transform.right * (moveInputs[3]-moveInputs[1]) * moveSpeed * Time.deltaTime;
        Vector3 verticalDis = Vector3.up * (moveInputs[4]-moveInputs[5]) * moveSpeed * Time.deltaTime;
        transform.position += horizontalDis + verticalDis;
    }

    void AcceleratedMove(){
        tempSpeed[0] = Accelerate(tempSpeed[0], movekeys[0], movekeys[2], moveSpeed, moveAcceleration);
        tempSpeed[1] = Accelerate(tempSpeed[1], movekeys[3], movekeys[1], moveSpeed, moveAcceleration);
        tempSpeed[2] = Accelerate(tempSpeed[2], movekeys[4], movekeys[5], moveSpeed, moveAcceleration);
        transform.position += Vector3.Scale(transform.forward, new Vector3(1,0,1)).normalized * tempSpeed[0] * Time.deltaTime
        + transform.right * tempSpeed[1] * Time.deltaTime + Vector3.up * tempSpeed[2] * Time.deltaTime;
    }

    /// <summary>
    /// 加速度的にspeedをコントロール
    /// </summary>
    /// <param name="key1">キーの割当。kay1-key2で正負判断</param>
    float Accelerate(float speed, KeyCode key1, KeyCode key2, float maxSpeed, float acceleration){
        float tempAcceleration = acceleration * Time.deltaTime;//そのフレームの加速量
        if(Input.GetKey(key1) && Input.GetKey(key2)){//両方押してるとき
            //何もしない
        }else if(Input.GetKey(key1)){//ポジティブだけ押してるとき
            if(speed < maxSpeed){
                speed += tempAcceleration;
            }
        }else if(Input.GetKey(key2)){//ネガティブだけ押してる
            if(speed > -maxSpeed){
                speed -= tempAcceleration;
            }
        }else{//該当方向の入力なし
            if(Mathf.Abs(speed) < tempAcceleration){
                speed = 0;
            }else if(speed > 0){
                speed -= tempAcceleration;
            }else{
                speed += tempAcceleration;
            }
        }
        return speed;
    }

    void Rotate(){
        int[] rotateInputs = {0,0,0,0};
        for(int i=0;i<4;i++){
            if(Input.GetKey(rotatekeys[i])){
                rotateInputs[i]++;
            }
        }
        transform.localEulerAngles += 
        new Vector3((rotateInputs[2]-rotateInputs[0]) * rotateSpeed * Time.deltaTime, (rotateInputs[3]-rotateInputs[1]) * rotateSpeed * Time.deltaTime, 0);
    }

    void AcceleratedRotate(){
        tempRotateSpeed[0] = Accelerate(tempRotateSpeed[0], rotatekeys[2], rotatekeys[0], rotateSpeed, rotateAcceleration);
        tempRotateSpeed[1] = Accelerate(tempRotateSpeed[1], rotatekeys[3], rotatekeys[1], rotateSpeed, rotateAcceleration);
        transform.localEulerAngles += new Vector3(tempRotateSpeed[0] * Time.deltaTime, tempRotateSpeed[1] * Time.deltaTime, 0);
    }
}
