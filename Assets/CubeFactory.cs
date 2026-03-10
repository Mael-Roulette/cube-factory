using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeFactory : MonoBehaviour
{
    private Stopwatch stopwatch;
    public int cooldown = 2000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    // Update is called once per frame
    void Update()
    {
         if (stopwatch.Elapsed.Milliseconds >= cooldown ) {
            CreateCube();
            stopwatch.Restart();
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
