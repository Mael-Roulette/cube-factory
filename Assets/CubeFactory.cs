using UnityEngine;

public class CubeFactory : MonoBehaviour
{
    public IsCollidingChecker isCollidingChecker;
    private bool _cubeSpawned = false;

    void Update()
    {
        if (isCollidingChecker.isColliding && !_cubeSpawned)
        {
            ChoiceInteract choice = FindFirstObjectByType<ChoiceInteract>();
            choice?.CreateCube();
            _cubeSpawned = true;
        }

        if (!isCollidingChecker.isColliding)
            _cubeSpawned = false;
    }
}