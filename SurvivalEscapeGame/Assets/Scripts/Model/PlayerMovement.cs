using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float Speed = 1;
    public float FastSpeed = 3;
    public KeyCode EnableFastSpeedWithKey = KeyCode.LeftShift;
    private Vector3 previousPosition = Vector3.zero;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        var currentSpeed = Speed;
        if (Input.GetKey(EnableFastSpeedWithKey)) {
            currentSpeed = FastSpeed;
        }
        var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        Vector3 currentPos = transform.position;
        previousPosition = currentPos;
    }
}