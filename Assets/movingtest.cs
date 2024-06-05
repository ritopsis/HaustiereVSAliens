using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class movingtest : NetworkBehaviour
{
    private void Update()
    {
        if (!IsOwner) return;
        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDirection.y = +1f;
        if (Input.GetKey(KeyCode.S)) moveDirection.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

        float moveSpeed = 3f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}

