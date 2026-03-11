using UnityEngine;

public class choiceInteract : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
