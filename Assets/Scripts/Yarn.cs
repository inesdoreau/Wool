using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yarn : MonoBehaviour
{
    public float maxSpeed;
    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;

    [HideInInspector]
    public Transform previousYarn;
    [HideInInspector]
    public YarnController controller;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetMaxSpeed();
        LinkYarn();
    }

    private void LinkYarn()
    {
        if (previousYarn != null)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, previousYarn.position);
        }
    }

    private void SetMaxSpeed()
    {
        if (_rigidbody.velocity.magnitude > maxSpeed)
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag.Equals("Player"))
    //    {
    //        controller.Rewind(this.gameObject);
    //    }
    //}
}
