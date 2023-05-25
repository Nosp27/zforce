using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;

    private Animator anim;

    private bool facingRight = true;

    private static readonly int Running = Animator.StringToHash("running");

    private Vector2 moveVector;
    private bool jump;
    private bool inputProcessed;

    [SerializeField] private float positionSyncThreshold = 0.01f;
    private NetworkVariable<Vector2> serverPosition = new NetworkVariable<Vector2>();

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        moveVector = Vector2.zero;
        jump = false;
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
        if (!IsServer)
        {
            MoveCommand(moveVector, jump);
            if (Vector2.Distance(transform.position, serverPosition.Value) > positionSyncThreshold)
            {
                transform.position = serverPosition.Value;
            }
        }
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
        MoveCommand(moveVector, jump);
        serverPosition.Value = transform.position;
    }

    void MoveCommand(Vector2 moveVector, bool jump)
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

        Bounds b = GetComponentInChildren<Collider2D>().bounds;
        Ray2D rayDown = new Ray2D(new Vector2(b.center.x, b.min.y), Vector3.down);
        Ray2D rayLeft = new Ray2D(new Vector2(b.max.x + 0.01f, b.center.y), Vector3.right);
        Ray2D rayRight = new Ray2D(new Vector2(b.min.x - 0.01f, b.center.y), Vector3.left);

        if (
            jump && (
                RaycastUtil.CheckRay(transform, rayDown) ||
                RaycastUtil.CheckRay(transform, rayLeft) ||
                RaycastUtil.CheckRay(transform, rayRight)
            )
        )
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Force);
        }
    }
}

public class RaycastUtil
{
    private static RaycastHit2D[] buffer = new RaycastHit2D[64];

    public static Collider2D RayHit(Transform transform, Ray2D ray)
    {
        int numHits = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, buffer, 0.1f);
        for (int i = 0; i < numHits; i++)
        {
            if (buffer[i].collider.transform.IsChildOf(transform))
                continue;
            return buffer[i].collider;
        }

        return null;
    }

    public static bool CheckRay(Transform transform, Ray2D ray)
    {
        return RayHit(transform, ray) != null;
    }
}