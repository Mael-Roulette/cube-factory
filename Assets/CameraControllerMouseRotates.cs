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

    private Rigidbody _rb;
    private Transform _bodyTrans;
    private Vector3 _moveInput = Vector3.zero;
    
    private CapsuleCollider _capsule;
    private float _standingHeight;
    private readonly float _crouchHeight = 1.0f;
    private readonly float _jumpHeight = 2.5f;
    private float _standingCenterY;

    private float _camTargetLocalY;
    private float _standingCamY;
    private float _crouchCamY = -0.6f;
    private float _jumpCamY = 1f;

    public bool AllReleased
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

        _bodyTrans = maincamTrans != null ? maincamTrans.parent : transform.parent;

        _rb = _bodyTrans.GetComponent<Rigidbody>();

        _capsule = _bodyTrans.GetComponent<CapsuleCollider>();
        _standingHeight = _capsule.height;
        _standingCenterY = _capsule.center.y;

        // Si "maincamTrans" n'a pas ?t? assign? dans l'?diteur on suppose que ce script est attach? ? la cam?ra elle-m?me.
        maincamTrans ??= transform;

        // You may need to adapt this keys to work with AZERTY keyboards
        //  You can replace Q with A and Z with W, or change the key set up to direction arrows (e.g., "Key.UpArrow")
        KeyCodesUsed = new Key[]
        {
            // Crouch down (move camera up/down)
            Key.LeftCtrl,
            // Jump 
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
            _camTargetLocalY = state ? _crouchCamY : _standingCamY;

            if (state)
            {
                // Accroupi : réduit la hauteur du collider vers le bas
                _capsule.height = _crouchHeight;
                _capsule.center = new Vector3(
                    _capsule.center.x,
                    _standingCenterY - (_standingHeight - _crouchHeight) / 2f,
                    _capsule.center.z
                );
            }
            else
            {
                // Debout : cast pour vérifier qu'il y a de la place au-dessus
                float castDist = (_standingHeight - _crouchHeight) / 2f;
                bool blocked = Physics.SphereCast(
                    _bodyTrans.position,
                    _capsule.radius * 0.9f,
                    Vector3.up,
                    out _,
                    castDist
                );

                if (!blocked)
                {
                    _capsule.height = _standingHeight;
                    _capsule.center = new Vector3(
                        _capsule.center.x,
                        _standingCenterY,
                        _capsule.center.z
                    );
                }
            }
        });
        //  Jumb
        InputAction[Key.Space].Add((state) =>
        {
            _camTargetLocalY = state ? _jumpCamY : _standingCamY;

            if (state)
            {
                // Accroupi : réduit la hauteur du collider vers le bas
                _capsule.height = _jumpHeight;
                _capsule.center = new Vector3(
                    _capsule.center.x,
                    _standingCenterY - (_standingHeight + _jumpHeight) / 2f,
                    _capsule.center.z
                );
            }
            else
            {
                // Debout : cast pour vérifier qu'il y a de la place au-dessus
                float castDist = (_standingHeight - _jumpHeight) / 2f;
                bool blocked = Physics.SphereCast(
                    _bodyTrans.position,
                    _capsule.radius * 0.9f,
                    Vector3.up,
                    out _,
                    castDist
                );

                if (!blocked)
                {
                    _capsule.height = _standingHeight;
                    _capsule.center = new Vector3(
                        _capsule.center.x,
                        _standingCenterY,
                        _capsule.center.z
                    );
                }
            }
        });
        InputAction[Key.W].Add(state => _moveInput.z = state ? 1 : 0);
        InputAction[Key.S].Add(state => _moveInput.z = state ? -1 : 0);
        InputAction[Key.A].Add(state => _moveInput.x = state ? -1 : 0);
        InputAction[Key.D].Add(state => _moveInput.x = state ? 1 : 0);
        
        Cursor.lockState = CursorLockMode.Locked;

        _standingCamY = maincamTrans.localPosition.y;
        _camTargetLocalY = _standingCamY;
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
    private readonly int maxCamWin = 4;
    private readonly Vector3[] camPosWin;
    private readonly Vector3[] camRotWin;

    private Vector2 mousePos = Vector2.zero;

    private void Update()
    {
        mousePos.x += Mouse.current.delta.x.ReadValue() * camRotSpeed * 2 * Time.deltaTime;
        mousePos.y -= Mouse.current.delta.y.ReadValue() * camRotSpeed * Time.deltaTime;
        mousePos.y = Mathf.Clamp(mousePos.y, -75, 75);

        maincamTrans.localRotation = Quaternion.Euler(mousePos.y, 0, 0);

        Vector3 camLocal = maincamTrans.localPosition;
        camLocal.y = Mathf.Lerp(camLocal.y, _camTargetLocalY, Time.deltaTime * 10f);
        maincamTrans.localPosition = camLocal;

        foreach (Key keycode in KeyCodesUsed)
        {
            bool isDown = Keyboard.current[keycode].isPressed;
            bool isUp = Keyboard.current[keycode].wasReleasedThisFrame;
            if (!isDown && !isUp) continue;
            isPressed[keycode] = isDown;
            if (InputAction.ContainsKey(keycode))
                foreach (inputCallback func in InputAction[keycode])
                    func(isPressed[keycode]);
        }
    }

    private void FixedUpdate()
    {
        // Rotation via Rigidbody pour ne pas conflicuer avec la physique
        _rb.MoveRotation(Quaternion.Euler(0, mousePos.x, 0));

        if (_moveInput == Vector3.zero) return;
        Vector3 move = (_bodyTrans.forward * _moveInput.z + _bodyTrans.right * _moveInput.x) * camTransSpeed;
        _rb.MovePosition(_rb.position + move * Time.fixedDeltaTime);
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
            mat = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
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
