using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_controller : MonoBehaviour
{

    // Use this for initialization
    public static int ballState;
    public static int whichRobo;
    public static int catchBuffer;
    public static float throwBuffer;  // 何秒後にボールを投げるか
    public static int throwBufferFlag = 1;
    public float transSpeed = 0.01f;
    private float throwSpeed = 0.0f;
    private float throwAngle = 0.0f;
    private float throwBackVelo = 0.0f; // 反動で後ろに進む速さ
    public static bool throwBackFlag = false; //後ろ向きに進んでる途中かどうか

    private float goalToThrow = 5.0f;
    private float startToThrow;
    private float directionToThrow = 1.0f;
    private float throwBufferCounter = 0.0f;

    public int turn = 0;  // 何ターンめか
    public List<string[]> SceneInfo;

    public GameObject Robo0;
    public GameObject Robo1;


    void Start()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        ballState = 0;  // 持っている
        whichRobo = 0;  // Robo0が
        catchBuffer = 0;

        SceneInfo = FileImport.ReadCSV("test");

    }

    // Update is called once per frame
    void Update()
    {
        if (throwBackVelo >= 0)
        {
            throwBackVelo = throwBackVelo - 0.05f * throwBackVelo - 0.001f;
            // Debug.Log(throwBackVelo);

            // ぶつかって後ろに下がるのを実装
            Vector3 pos;
            if (whichRobo == 0)
            {
                pos = Robo0.transform.position;
                pos.x -= throwBackVelo;
                Robo0.transform.position = pos;

                if (ballState != 1)
                {
                    Transform myTransform = this.transform;
                    Vector3 posBall = myTransform.position;
                    posBall.x -= throwBackVelo;    // 直上へ0.01加算
                    myTransform.position = posBall;  // 座標を設定
                }

            }
            else
            {
                pos = Robo1.transform.position;
                pos.x += throwBackVelo;
                Robo1.transform.position = pos;

                if (ballState != 1)
                {
                    Transform myTransform = this.transform;
                    Vector3 posBall = myTransform.position;
                    posBall.x += throwBackVelo;    // 直上へ0.01加算
                    myTransform.position = posBall;  // 座標を設定
                }
            }

        }
        else
        {
            throwBackVelo = 0;
            if (throwBackFlag)
            {
                Debug.Log("throw back end");
                Debug.Log("throw buffer flag" + throwBufferFlag.ToString());
                throwBackFlag = false;



                // throwBuffer = 3.0f;  // 反動が止まった後何秒後に投げるか
                //throwBuffer = Random.Range(0.05f, 2.0f);
                throwBuffer = float.Parse(SceneInfo[turn][1]);
                throwBufferCounter = 0.0f;
                Debug.Log("will throw after second " + throwBuffer.ToString());

                if (whichRobo == 0)
                {
                    startToThrow = Robo0.transform.position.x;
                    goalToThrow = -5.0f;
                    directionToThrow = 1.0f;
                }
                else
                {
                    startToThrow = Robo1.transform.position.x;
                    goalToThrow = 5.0f;
                    directionToThrow = 1.0f;
                }

            }
                
        }


        if (ballState == 0)
        {
            if (throwBufferFlag == 0 && throwBackFlag == false)
            {
                if (throwBuffer > throwBufferCounter)
                {
                    throwBufferCounter += Time.deltaTime;


                    Vector3 pos;
                    if (whichRobo == 0)
                        pos = Robo0.transform.position;
                    else
                        pos = Robo1.transform.position;

                    pos.x += directionToThrow * 2.0f * (goalToThrow - startToThrow) / (throwBuffer* throwBuffer) * throwBufferCounter * Time.deltaTime;
                    
                    if (whichRobo == 0)
                        Robo0.transform.position = pos;
                    else
                        Robo1.transform.position = pos;

                    Transform myTransform = this.transform;
                    Vector3 posBall = myTransform.position;
                    posBall.x += directionToThrow * 2.0f * (goalToThrow - startToThrow) / (throwBuffer * throwBuffer) * throwBufferCounter * Time.deltaTime;
                    myTransform.position = posBall;  // 座標を設定


                }
                else
                {
                    throwBufferFlag = 1;
                    Debug.Log("got ready to throw");
                }
                

            }

            if (throwBufferFlag == 1)
            {
                // 投げる強さ: 既知で与える<====================================================================================CSVから読み込めるようにする
                throwSpeed = float.Parse(SceneInfo[turn][0]);//9.4f;
                throwSpeed = throwSpeed + Random.Range(0, 8.0f);
                Debug.Log("throw speed " + throwSpeed.ToString());
                float x_0 = Robo0.transform.position.x;
                float x_1 = Robo1.transform.position.x;
                float gravit = -1 * Physics.gravity.y;
                float temp = gravit * (x_1 - x_0 - 1.0f - 1.0f) / (throwSpeed * throwSpeed); // - 1.0fはロボットのサイズの分- 1.0fはボールの大きさの分
                //float temp2 = (temp - x_0) / Mathf.Sqrt(1 + x_1 * x_1);

                // Debug.Log(temp);
                if (temp > 1.0)
                {
                    throwAngle = 45;
                }
                else
                {
                    throwAngle = Mathf.Asin(temp) * 0.5f * 180f / Mathf.PI;
                    if (bool.Parse(SceneInfo[turn][2]))
                    {
                        throwAngle = 90 - throwAngle;
                        throwAngle = throwAngle;
                    }

                }
                if (whichRobo == 1)
                {
                    throwAngle = 180 - throwAngle;
                }
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                transform.eulerAngles = new Vector3(-1 * throwAngle, 90, 0);
                // gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwSpeed);

                Rigidbody rb = this.transform.GetComponent<Rigidbody>();
                rb.velocity = transform.forward * throwSpeed;
                //Debug.Log(rb.velocity.magnitude);

                ballState = 1;
                throwBufferFlag = 0;

                //次のターンを呼び出し
                turn += 1;
                Debug.Log(SceneInfo[turn][0]);

            }
        }
      
        if (ballState == 2)
        {
            if (catchBuffer == 0)
            {
                catchBuffer = 1;
                transSpeed = 0.1f;//Random.Range(0.05f, 0.2f);
            }
            if (catchBuffer == 1)
            {
                Transform myTransform = this.transform;
                Vector3 posBall = myTransform.position;
                //Debug.Log("going up! " + this.transform.position.y.ToString());
                posBall.y += transSpeed;    // 直上へ0.01加算
                myTransform.position = posBall;  // 座標を設定
                //Debug.Log("going up after! " + this.transform.position.y.ToString());

                if (posBall.y >= 4.0)
                {
                    Debug.Log("here to throw!");
                    posBall.y -= transSpeed * 0.5f;// = 4.0f;
                    myTransform.position = posBall;

                    ballState = 0;
                    catchBuffer = 0;
                    //throwBuffer = Random.Range(10, 100);

                }
            }


        }



    }

    void OnCollisionEnter(Collision collision)
    {
        if (ballState == 1)
        {
            if (whichRobo == 0)
            {
                if (collision.gameObject.name == "Robo1")
                {
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    whichRobo = 1;
                    ballState = 2;
                    throwBackVelo = throwSpeed * Mathf.Abs(Mathf.Cos(throwAngle * Mathf.PI / 180.0f)) * 0.01f;
                }
            }
            else if (whichRobo == 1)
            {
                if (collision.gameObject.name == "Robo0")
                {
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    whichRobo = 0;
                    ballState = 2;
                    throwBackVelo = throwSpeed * Mathf.Abs(Mathf.Cos(throwAngle * Mathf.PI / 180.0f)) * 0.01f;
                }
            }

            if (collision.gameObject.name == "Robo1" || collision.gameObject.name == "Robo0")
            {
                Debug.Log("receive speed" + throwBackVelo.ToString());
                throwBackFlag = true;
            }


        }

    }

}
