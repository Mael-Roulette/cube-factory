using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeFactory : MonoBehaviour
{

    public IsCollidingChecker isCollidingChecker;

    void Update()
    {
         if (isCollidingChecker.isColliding) {
            CreateCube();
        }
    }

    public GameObject CreateCube()
    {
        GameObject cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Transform cubeTrans = cubeGo.transform;

        cubeTrans.position = new Vector3(-1.5f, 1.3f, -1.8f);
        cubeTrans.localScale = new Vector3(.15f,.15f,.15f);

        cubeGo.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        cubeGo.AddComponent<Moving>();

        Rigidbody cubeRB = cubeGo.AddComponent<Rigidbody>();
        cubeRB.linearVelocity = new Vector3(Random.value * .1f,-1,Random.value * .1f) * 5f;

        return cubeGo;
    }
}
