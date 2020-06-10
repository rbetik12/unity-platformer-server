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
    private bool isFloorColliding;
    private int jumpsAmount;

    private const float MovementSpeed = 1000f;
    private const float MobileMovementSpeedMultiplier = 1.5f;
    private const float MaxVelocity = 4.5f;
    private const float MinVelocity = -MaxVelocity;

    private void Start() {
        gravityScale = rb.gravityScale;
    }

    public void FixedUpdate() {
        isFloorColliding = IsFloorColliding();
        CheckJump();
        Move();
        SendPosition();
        Debug.Log($"Colliding floor: {isFloorColliding}\n\tJumps amount: {jumpsAmount}");
    }

    private void CheckJump() {
        if (isJumping && jumpsAmount < 1) {
            jumpsAmount += 1;
        }
        else {
            isJumping = false;
        }
    }

    private void Move() {
        if (isFloorColliding && horizontalInput.Equals(0) && !isJumping) {
            rb.velocity = Vector2.zero;
        }
        else {
            if (!horizontalInput.Equals(0)) {
                rb.AddForce(new Vector2(horizontalInput * (gravityScale * Time.fixedDeltaTime * MovementSpeed * MobileMovementSpeedMultiplier), 0));
            }

            if (isJumping) {
                rb.AddForce(Vector2.up * (gravityScale * Time.fixedDeltaTime * MovementSpeed * 20f));
                isJumping = false;
            }
        }
        Vector2 clampedVelocity = Vector2.zero;
        clampedVelocity.x = Mathf.Clamp(rb.velocity.x, MinVelocity, MaxVelocity);
        clampedVelocity.y = rb.velocity.y;
        rb.velocity = clampedVelocity;
    }
    private bool IsFloorColliding() {
        const float dist = 0.1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f),
            Vector2.down, dist);
        if (raycastHit.collider == null) return false;
        bool isNotTrigger = !raycastHit.collider.isTrigger;
        if (isNotTrigger)
            jumpsAmount = 0;
        return isNotTrigger;
    }

    private void SendPosition() {
        ServerSend.PlayerPosition(this);
    }

    public void SetInput(float horizontal, float jump) {
        isJumping = Mathf.Abs(1 - jump) < Single.Epsilon;

        horizontalInput = horizontal;
    }
    

    public void Initialize(int id, string username) {
        this.id = id;
        this.username = username;
    }
}