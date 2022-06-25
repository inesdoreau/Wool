using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class YarnController : MonoBehaviour
{
    [SerializeField]
    PlayerController woolBall;
    [SerializeField]
    GameObject yarnPrefab;
    [SerializeField]
    GameObject parentObject;
    [SerializeField]
    GameObject startPoint;
    

    List<GameObject> yarnRope = new List<GameObject>();
    GameObject wool;
    private AssetsInputs _input;

    [SerializeField]
    [Range(1, 1000)]
    int lenght = 1;
    int yarnNumber = 0;

    [SerializeField]
    float yarnDistance = 0.21f;

    [SerializeField]
    bool reset; 
    [SerializeField]
    bool spawn; 
    [SerializeField]
    bool snapFirst; 
    [SerializeField]
    bool snapLast;

    [SerializeField]
    bool block; 
    [SerializeField]
    bool rewind;

    public float RewindTimeout = 0.50f;
    private float _rewindTimeoutDelta;


    private void Start()
    {         
        CreateWool();
        CreateStartPoint();
        _rewindTimeoutDelta = RewindTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            foreach(GameObject yarn in yarnRope)
            {
                Destroy(yarn);
            }
        }

        if (spawn)
        {
           AddYarn();
            //Spawn();
            spawn = false;
        }

        AddYarn();
    }

    private void CreateStartPoint()
    {
        GameObject firstYarn;
        firstYarn = Instantiate(yarnPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
        firstYarn.transform.eulerAngles = new Vector3(90, 0, 90);
        firstYarn.name = parentObject.transform.childCount.ToString();
        firstYarn.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        firstYarn.GetComponent<HingeJoint>().connectedBody = startPoint.GetComponent<Rigidbody>();       
        yarnRope.Add(firstYarn);

        ConnectLastYarn();
    }

    private void CreateWool()
    {
        wool = Instantiate(woolBall.gameObject, new Vector3(transform.position.x + yarnDistance, transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
        wool.gameObject.name = parentObject.transform.childCount.ToString();

        _input = wool.GetComponent<AssetsInputs>();
    }

    private void ConnectLastYarn()
    {
        wool.GetComponent<SpringJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<Rigidbody>();
    }

    public void AddYarn()
    {
        if (_input.rewind && yarnRope.Count != 1)
        {
            if(_rewindTimeoutDelta >= 0)
            {
                _rewindTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                GameObject lastYarn = yarnRope[yarnRope.Count - 1];
                wool.transform.position =  lastYarn.transform.position; 
                yarnRope.Remove(lastYarn);
                Destroy(lastYarn);            
                wool.GetComponent<SpringJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
                _rewindTimeoutDelta = RewindTimeout;
            }            
            return;
        }

        if (_input.block)
            return;

        Transform previousYarnTf = yarnRope[yarnRope.Count - 1].transform;
        Vector3 distance = previousYarnTf.position - wool.transform.position;

        if (distance.magnitude > yarnDistance)
        {
            GameObject tmp;
            tmp = Instantiate(yarnPrefab, new Vector3(wool.transform.position.x, wool.transform.position.y, wool.transform.position.z), Quaternion.identity, parentObject.transform);
            //tmp.transform.forward = distance/distance.magnitude ;

            tmp.name = parentObject.transform.childCount.ToString();
            tmp.GetComponent<HingeJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount-1).ToString()).GetComponent<Rigidbody>();
           
            ConnectLastYarn();
            yarnRope.Add(tmp);
        }

        
    }

    public void Spawn()
    {
        int count = (int)(lenght / yarnDistance);

        for(int i = 0; i <= count; i++)
        {
            if(i < count)
                {
                GameObject tmp;
                tmp = Instantiate(yarnPrefab, new Vector3(transform.position.x + yarnDistance * (i + 1), transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
                
                tmp.transform.eulerAngles = new Vector3(90, 0, 90);
                tmp.name = parentObject.transform.childCount.ToString();

                if (i == 0)
                {

                    tmp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    tmp.GetComponent<HingeJoint>().connectedBody = startPoint.GetComponent<Rigidbody>();
                }
                else
                {
                    tmp.GetComponent<HingeJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
                }
            
                yarnRope.Add(tmp);

            }            
            else
            {
                GameObject wool;
                //wool = Instantiate(woolBall, new Vector3(transform.position.x + yarnDistance * (i + 1), transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
                //wool.name = parentObject.transform.childCount.ToString();
                //yarnRope.Add(wool);
                //wool.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                //wool.GetComponent<HingeJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
                //tmp.GetComponent<HingeJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }

        }
        
    }
}
