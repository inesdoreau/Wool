using System;
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

    [SerializeField]
    int yarnSize;
    

    List<GameObject> yarnRope = new List<GameObject>();
    GameObject wool;
    private AssetsInputs _input;

    [SerializeField]
    float yarnDistance = 0.1f;

    private void Start()
    {         
        CreateWool();
        CreateStartPoint();
    }

    // Update is called once per frame
    void Update()
    {      
        AddYarn();
    }

    private void CreateStartPoint()
    {
        GameObject firstYarn;
        firstYarn = Instantiate(yarnPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
        firstYarn.transform.eulerAngles = new Vector3(90, 0, 90);
        firstYarn.name = parentObject.transform.childCount.ToString();
        firstYarn.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        firstYarn.GetComponent<CharacterJoint>().connectedBody = startPoint.GetComponent<Rigidbody>();       
        yarnRope.Add(firstYarn);

        ConnectLastYarn();
    }

    private void CreateWool()
    {
        wool = Instantiate(woolBall.gameObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
        wool.gameObject.name = parentObject.transform.childCount.ToString();

        _input = wool.GetComponent<AssetsInputs>();
    }

    private void ConnectLastYarn()
    {
        wool.GetComponent<SpringJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount).ToString()).GetComponent<Rigidbody>();
    }

    public void Rewind(GameObject yarnSegmentToRewind)
    {
        //if (_rewindTimeoutDelta >= 0)
        //{
        //    _rewindTimeoutDelta -= Time.deltaTime;
        //    wool.transform.position = Vector3.Lerp(wool.transform.position, yarnSegmentToRewind.transform.position, RewindTimeout);
        //}
        //else
        //{
        //    wool.transform.position = yarnSegmentToRewind.transform.position;

        Debug.Log(yarnRope.IndexOf(yarnSegmentToRewind), yarnSegmentToRewind);
        if (yarnRope.IndexOf(yarnSegmentToRewind) == yarnRope.Count - 1)
        {

            //wool.GetComponent<SpringJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            wool.GetComponent<SpringJoint>().connectedBody = yarnSegmentToRewind.GetComponent<CharacterJoint>().connectedBody;
            //wool.transform.position = yarnSegmentToRewind.transform.position;

            yarnRope.Remove(yarnSegmentToRewind); 
            Destroy(yarnSegmentToRewind);        
        }

        //_rewindTimeoutDelta = RewindTimeout;
        //}
    }



    public void AddYarn()
    {
        //if (_input.rewind && yarnRope.Count != 1)
        //{
        //    GameObject lastYarn = yarnRope[yarnRope.Count - 1];
        //    Rewind(lastYarn);        
        //    return;
        //}

        if (_input.block || parentObject.transform.childCount == yarnSize)
        {
            wool.GetComponent<PlayerController>().Block(true);
            return; 
        }
        wool.GetComponent<PlayerController>().Block(false);        

        Transform previousYarnTf = yarnRope[yarnRope.Count - 1].transform;
        Vector3 distance = previousYarnTf.position - wool.transform.position;
        
        if (distance.magnitude > yarnDistance)
        {
            GameObject tmp;
            tmp = Instantiate(yarnPrefab, wool.transform.position , Quaternion.identity, parentObject.transform);

            tmp.name = parentObject.transform.childCount.ToString();
            tmp.GetComponent<CharacterJoint>().connectedBody = yarnRope[yarnRope.Count - 1].GetComponent<Rigidbody>();
                //parentObject.transform.Find((parentObject.transform.childCount-1).ToString()).GetComponent<Rigidbody>();
            tmp.GetComponent<Yarn>().controller = this;
            tmp.GetComponent<Yarn>().previousYarn = previousYarnTf;

            ConnectLastYarn();
            yarnRope.Add(tmp);
        }


    }

    
}
