﻿using UnityEngine;

public class DroneCamera : MonoBehaviour
{
    public Camera myCamera;

    private float forwardSpeed = 6f;
    private float backwardSpeed = 4f;
    private float sideSpeed = 4f;
    private float downSpeed = 8f;
    private float upSpeed = 4f;

    private float multyplier = 1f;

    private void Start()
    {
        this.myCamera = gameObject.GetComponent<Camera>();
        this.forwardSpeed *= this.multyplier;
        this.backwardSpeed *= this.multyplier;
        this.downSpeed *= this.multyplier;
        this.upSpeed *= this.multyplier;
        this.sideSpeed *= this.multyplier;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            this.transform.position += this.transform.forward * this.forwardSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            this.transform.position -= this.transform.forward * this.backwardSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            this.transform.position -= this.transform.right * this.sideSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            this.transform.position += this.transform.right * this.sideSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.position += this.transform.up * this.upSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.position -= this.transform.up * this.downSpeed * Time.deltaTime;
        }
    }
}

