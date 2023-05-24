using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float selfDestructTimeout = 1f;
    void Start()
    {
        Destroy(gameObject, selfDestructTimeout);
    }
}
