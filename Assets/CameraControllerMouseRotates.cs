using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControllerMouseRotates : MonoBehaviour
{
    private static CameraControllerMouseRotates _instance;
    public static CameraControllerMouseRotates instance => _instance;

    public delegate void inputCallback(bool pressed);

    private Key[] KeyCodesUsed;
    public Dictionary<Key, bool> isPressed = new Dictionary<Key, bool>(); // Pressed, Released events
    private Dictionary<Key, List<inputCallback>> InputAction = new Dictionary<Key, List<inputCallback>>();

    public const float camYpos = 1.8f;
    public float camRotSpeed = 10f;
    public float camTransSpeed = 10f;

    // Set this as the camera's transform in the editor
    public Transform maincamTrans;
    
    public bool allReleased
    {
        get
        {
            foreach (Key keycode in KeyCodesUsed)
            {
                if (isPressed[keycode]) return false;
            }
            return true;
        }
    }

    private void Start()
    {
        _instance = this;

        // Si "maincamTrans" n'a pas ?t? assign? dans l'?diteur on suppose que ce script est attach? ? la cam?ra elle-m?me.
        if (maincamTrans ==  null)
        {
            maincamTrans = transform;
        }

        maincamTrans.position = new Vector3(0, camYpos, 0);
        maincamTrans.rotation = Quaternion.identity;
        maincamTrans.Rotate(Vector3.right, 20);
        
        camPosWin = new Vector3[maxCamWin];
        camRotWin = new Vector3[maxCamWin];
        for (int i = 0; i < maxCamWin; i++)
        {
            camPosWin[i].y = camYpos;
            // camRotWin[i].x = -10f;
        }

        // You may need to adapt this keys to work with AZERTY keyboards
        //  You can replace Q with A and Z with W, or change the key set up to direction arrows (e.g., "Key.UpArrow")
        KeyCodesUsed = new Key[]
        {
            // Crouch down (move camera up/down)
            Key.LeftCtrl,
            // Jump (move camera up)
            Key.Space,
            // Move forward
            Key.W,
            // Move backward
            Key.S,
            // Strafe left
            Key.A,
            // Strafe right
            Key.D,
        };

        foreach (Key keycode in KeyCodesUsed)
        {
            isPressed.Add(keycode, false);
            InputAction.Add(keycode, new List<inputCallback>());
        }
        
        // Define input actions/effects

        //  Crouch
        InputAction[Key.LeftCtrl].Add((state) =>
        {
            Vector3 camPos = maincamTrans.position;
            camPos.y = state ? .6f: camYpos;
            maincamTrans.position = camPos;
        });
        //  Jump
        InputAction[Key.Space].Add((state) =>
        {
            Vector3 camPos = maincamTrans.position;
            // TODO : Revoir le saut
            camPos.y = state ? .6f: camYpos;
            maincamTrans.position = camPos;
        });
        //  Move forward
        InputAction[Key.W].Add(state =>
        {
            if (state)
            {
                Vector3 oldPos = maincamTrans.position;
                float Ypos = oldPos.y; // Keep Y position that can bee .6f or camYpos
                Vector3 newPos = oldPos + maincamTrans.forward * camTransSpeed * Time.deltaTime;
                newPos.y = Ypos;
                maincamTrans.position = newPos;
            }
        });
        //  Move backward
        InputAction[Key.S].Add(state =>
        {
            if (state)
            {
                Vector3 oldPos = maincamTrans.position;
                float Ypos = oldPos.y;
                Vector3 newPos = oldPos - maincamTrans.forward * camTransSpeed * Time.deltaTime;
                newPos.y = Ypos;
                maincamTrans.position = newPos;
            }
        });
        //  Strafe left
        InputAction[Key.A].Add(state =>
        {
            if (state)
            {
                Vector3 oldPos = maincamTrans.position;
                float Ypos = oldPos.y;
                Vector3 newPos = oldPos - maincamTrans.right * camTransSpeed * Time.deltaTime;
                newPos.y = Ypos;
                maincamTrans.position = newPos;
            }
        });
        //  Strafe right
        InputAction[Key.D].Add(state =>
        {
            if (state)
            {
                Vector3 oldPos = maincamTrans.position;
                float Ypos = oldPos.y;
                Vector3 newPos = oldPos + maincamTrans.right * camTransSpeed * Time.deltaTime;
                newPos.y = Ypos;
                maincamTrans.position = newPos;
            }
        });
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    // Smoother camera control with averaging window
    Vector3 getAverageVec3(Vector3[] arr)
    {
        Vector3 outvec = Vector3.zero;
        for (int i = 0; i < maxCamWin; i++)
        {
            outvec += arr[i];
        }
        return outvec / maxCamWin;
    }
    private int iCamWin = 0;
    private int maxCamWin = 4;
    private Vector3[] camPosWin;
    private Vector3[] camRotWin;

    private Vector2 mousePos = Vector2.zero;

    private void Update()
    {
        // Mouse movement -> camera rotation
        mousePos.x += Mouse.current.delta.x.ReadValue() * camRotSpeed * 2 * Time.deltaTime;
        mousePos.y -= Mouse.current.delta.y.ReadValue() * camRotSpeed * Time.deltaTime;
        mousePos.y = Mathf.Clamp(mousePos.y, -75, 75);
        Vector3 newCamRot = new Vector3(mousePos.y, mousePos.x, 0f);
        
        foreach (Key keycode in KeyCodesUsed)
        {
            // bool hasChanged = false;
            bool isDown = Keyboard.current[keycode].isPressed;
            bool isUp = Keyboard.current[keycode].wasReleasedThisFrame;

            if (!isDown && !isUp) continue;

            isPressed[keycode] = isDown;

            // Continue to invoke attached action(s) as long as the key is pressed
            if (InputAction.ContainsKey(keycode))
            {
                foreach (inputCallback func in InputAction[keycode])
                {
                    func(isPressed[keycode]);
                }
            }
        }

        Vector3 newCamPos = maincamTrans.position;
        // Force user to stay within a square centered at the origin (x: 0, z: 0)
        //newCamPos.x = Mathf.Clamp(newCamPos.x, -1.85f, 1.85f);
        //newCamPos.z = Mathf.Clamp(newCamPos.z, -2f, 2f);
        
        iCamWin = (++iCamWin) % maxCamWin;

        maincamTrans.position = (getAverageVec3(camPosWin) + newCamPos) / 2;
        maincamTrans.eulerAngles = (getAverageVec3(camRotWin) - new Vector3(90f, 0, 0) + newCamRot) / 2;

        camPosWin[iCamWin] = newCamPos;
        camRotWin[iCamWin] = newCamRot + new Vector3(90f, 0, 0);
    }
    
    private void OnEnable()
    {
        UnityEngine.Rendering.RenderPipelineManager.endCameraRendering += DrawCrosshair;
    }

    private void OnDisable()
    {
        UnityEngine.Rendering.RenderPipelineManager.endCameraRendering -= DrawCrosshair;
    }

    // Draws a cursor in the middle of the screen
    private Material mat;
    private void DrawCrosshair(UnityEngine.Rendering.ScriptableRenderContext ctx, Camera cam)
    {
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MonoBehaviour.OnPostRender.html
        
        if (!mat)
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            // Turn off backface culling, depth writes, depth test.
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        GL.PushMatrix();
        GL.LoadOrtho();
        
        float screenRatio = (float)Screen.height / Screen.width;
        float length = .012f, thickness = .0015f;

        mat.SetPass(0);
        // draw a quad over whole screen
        GL.Begin(GL.QUADS);
        GL.Color(Color.green);
        GL.Vertex3(.5f-length, .5f-thickness,0);
        GL.Vertex3(.5f+length, .5f-thickness, 0);
        GL.Vertex3(.5f+length, .5f+thickness, 0);
        GL.Vertex3(.5f-length, .5f+thickness, 0);
        GL.End();

        length /= screenRatio;
        thickness *= screenRatio;
        
        GL.Begin(GL.QUADS);
        GL.Color(Color.green);
        GL.Vertex3(.5f-thickness, .5f-length, 0);
        GL.Vertex3(.5f-thickness, .5f+length, 0);
        GL.Vertex3(.5f+thickness, .5f+length, 0);
        GL.Vertex3(.5f+thickness, .5f-length, 0);
        GL.End();

        GL.PopMatrix();
    }
}
