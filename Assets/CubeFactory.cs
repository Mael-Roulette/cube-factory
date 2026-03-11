using UnityEngine;

public class CubeFactory : MonoBehaviour
{
    public IsCollidingChecker isCollidingChecker;
    private bool _cubeSpawned = false;

    void Update()
    {
        if (isCollidingChecker.isColliding && !_cubeSpawned)
        {
            CreateCube();
            _cubeSpawned = true;
        }

        if (!isCollidingChecker.isColliding)
            _cubeSpawned = false;
    }

    public GameObject CreateCube()
    {
        GameObject cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Transform cubeTrans = cubeGo.transform;

        cubeTrans.position = new Vector3(-1.5f, 1.3f, -1.8f);
        cubeTrans.localScale = new Vector3(.15f, .15f, .15f);

        // Utilise la couleur du bouton sélectionné
        cubeGo.GetComponent<MeshRenderer>().material.color = ColorInteract.selectedColor;
        cubeGo.AddComponent<Moving>();

        Rigidbody cubeRB = cubeGo.AddComponent<Rigidbody>();
        cubeRB.linearVelocity = new Vector3(Random.value * .1f, -1, Random.value * .1f) * 5f;

        return cubeGo;
    }
}