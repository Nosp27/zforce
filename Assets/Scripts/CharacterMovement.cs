using Unity.Netcode;
using UnityEngine;

public class CharacterMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;

    private Animator anim;

    private bool facingRight = true;

    private static readonly int Running = Animator.StringToHash("running");

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        bool jump = false;
        Vector2 moveVector = Vector2.zero;
        if (Input.GetKey(KeyCode.D))
        {
            moveVector = Vector2.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveVector = -Vector2.right;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            jump = true;
        }

        MoveCommandServerRpc(moveVector, jump);
    }

    void LookRight(bool right)
    {
        facingRight = right;
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y = facingRight ? 0 : 180;
        transform.rotation = Quaternion.Euler(rot);
    }

    [ServerRpc]
    void MoveCommandServerRpc(Vector2 moveVector, bool jump)
    {
        rb.velocity = new Vector2(moveVector.x * moveSpeed, rb.velocity.y);
        if (moveVector != Vector2.zero)
        {
            LookRight(moveVector.x > 0);
            anim.SetBool(Running, true);
        }
        else
        {
            anim.SetBool(Running, false);
        }

        if (jump && (CheckFloor() || CheckWall()))
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Force);
        }
    }

    bool CheckFloor()
    {
        Bounds b = GetComponentInChildren<Collider2D>().bounds;
        return Physics2D.Raycast(new Vector2(b.center.x, b.min.y), Vector3.down, 0.1f);
    }

    bool CheckWall()
    {
        Bounds b = GetComponentInChildren<Collider2D>().bounds;
        return Physics2D.Raycast(new Vector2(b.max.x + 0.01f, b.center.y), Vector3.right, 0.1f) ||
               Physics2D.Raycast(new Vector2(b.min.x - 0.01f, b.center.y), Vector3.left, 0.1f);
    }
}