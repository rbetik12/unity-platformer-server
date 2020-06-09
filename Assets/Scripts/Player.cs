using System;
using System.Globalization;
using Net;
using UnityEngine;

public class Player : MonoBehaviour {
    public int id;
    public string username;
    public Rigidbody2D rb;
    public Quaternion rotation;

    private float horizontalInput;
    private bool isMoving;
    private float gravityScale;
    private bool isJumping;

    private const float MovementSpeed = 1000f;
    private const float MobileMovementSpeedMultiplier = 1.5f;
    private const float MaxVelocity = 4.5f;
    private const float MinVelocity = -MaxVelocity;

    private void Start() {
        gravityScale = rb.gravityScale;
    }

    public void FixedUpdate() {
        Move();
        SendPosition();
    }

    private void Move() {
        if (isMoving) {
            if (!horizontalInput.Equals(0)) {
                Vector2 velocity =
                    new Vector2(
                        horizontalInput * (gravityScale * Time.fixedDeltaTime * MovementSpeed *
                                           MobileMovementSpeedMultiplier), 0);
                rb.AddForce(velocity);
            }

            if (isJumping) {
                rb.AddForce(Vector2.up * (gravityScale * Time.fixedDeltaTime * MovementSpeed * 20f));
                isJumping = false;
                Debug.Log("Jumped");
            }
        }
        else {
            rb.velocity = Vector2.zero;
        }
        Vector2 clampedVelocity = new Vector2();
        clampedVelocity.x = Mathf.Clamp(rb.velocity.x, MinVelocity, MaxVelocity);
        clampedVelocity.y = rb.velocity.y;
        rb.velocity = clampedVelocity;
    }

    private void SendPosition() {
        ServerSend.PlayerPosition(this);
    }

    public void SetInput(float horizontal, float isMoving, float isJumping) {
        this.isMoving = Mathf.Abs(1 - isMoving) < Single.Epsilon;
        this.isJumping = Mathf.Abs(1 - isJumping) < Single.Epsilon;

        horizontalInput = horizontal;
        // Debug.Log($"Is moving: {this.isMoving}, Joystick: {horizontalInput}");
        // Debug.Log($"Jump: {this.isJumping}");
    }
    

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
    }
}