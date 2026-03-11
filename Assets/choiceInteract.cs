using UnityEngine;

public class choiceInteract : MonoBehaviour
{
    private static int _selectedIndex = 0;
    public static int selectedIndex => _selectedIndex;

    public Transform modelParent;

    // Le plateau de prévisualisation (Cylinder)
    public Transform previewParent;

    private GameObject _previewInstance;

    private void Start()
    {
        RefreshPreview();
    }

    public void Next()
    {
        if (modelParent == null) return;
        _selectedIndex = (_selectedIndex + 1) % modelParent.childCount;
        RefreshPreview();
    }

    public void Prev()
    {
        if (modelParent == null) return;
        _selectedIndex = (_selectedIndex - 1 + modelParent.childCount) % modelParent.childCount;
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        if (modelParent == null || previewParent == null) return;

        // Détruire l'ancienne prévisualisation
        if (_previewInstance != null)
            Destroy(_previewInstance);

        // Cloner le modèle sélectionné sur le plateau
        GameObject source = modelParent.GetChild(_selectedIndex).gameObject;
        _previewInstance = Instantiate(source, previewParent);
        _previewInstance.transform.localPosition = Vector3.zero;
        _previewInstance.transform.localRotation = Quaternion.identity;
        _previewInstance.transform.localScale = source.transform.localScale;

        // Désactiver les colliders sur le visuel
        foreach (var col in _previewInstance.GetComponents<Collider>())
            col.enabled = false;
    }

    public GameObject CreateCube()
    {
        if (modelParent == null) return null;

        GameObject source = modelParent.GetChild(_selectedIndex).gameObject;
        GameObject cubeGo = Instantiate(source);

        cubeGo.transform.position = new Vector3(-1.5f, 1.3f, -1.8f);
        cubeGo.GetComponent<MeshRenderer>().material.color = ColorInteract.selectedColor;
        cubeGo.AddComponent<Moving>();

        Rigidbody cubeRB = cubeGo.AddComponent<Rigidbody>();
        cubeRB.linearVelocity = new Vector3(Random.value * .1f, -1, Random.value * .1f) * 5f;

        return cubeGo;
    }
}