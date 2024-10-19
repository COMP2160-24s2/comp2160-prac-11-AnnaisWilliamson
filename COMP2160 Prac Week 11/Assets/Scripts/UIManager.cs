/**
 * A singleton class to allow point-and-click movement of the marble.
 * 
 * It publishes a TargetSelected event which is invoked whenever a new target is selected.
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;
using UnityEngine.InputSystem;

// note this has to run earlier than other classes which subscribe to the TargetSelected event
[DefaultExecutionOrder(-100)]
public class UIManager : MonoBehaviour
{
#region UI Elements
    [SerializeField] private Transform crosshair;
    [SerializeField] private Transform target;

    public LayerMask wallsLayer;

    [SerializeField] private Transform groundTarget;
    private Plane groundPlane;

    #endregion

    #region Singleton
    static private UIManager instance;
    static public UIManager Instance
    {
        get { return instance; }
    }
#endregion 

#region Actions
    private Actions actions;
    private InputAction mouseAction;
    private InputAction deltaAction;
    private InputAction selectAction;
    private InputAction zoomAction;
#endregion

#region Events
    public delegate void TargetSelectedEventHandler(Vector3 worldPosition);
    public event TargetSelectedEventHandler TargetSelected;
#endregion

#region Init & Destroy
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one UIManager in the scene.");
        }

        instance = this;

        actions = new Actions();
        mouseAction = actions.mouse.position;
        deltaAction = actions.mouse.delta;
        selectAction = actions.mouse.select;

        zoomAction = actions.camera.zoom;

        Cursor.visible = false;
        target.gameObject.SetActive(false);

        //later the plane position so it sits above the ground layer
        //since grounds y is -0.5, it needs to get above it.
        Vector3 groundPosition = groundTarget.position + (Vector3.up * 1f);
        
        //Create a new plane with normal (0, 1, 0),
        //its at the same postion as ground but slightly above
        //so the marble stops falling throuigh the floor
        groundPlane = new Plane(Vector3.up, groundPosition);
    }

    void OnEnable()
    {
        actions.mouse.Enable();
        actions.camera.Enable();
    }

    void OnDisable()
    {
        actions.mouse.Disable();
        actions.camera.Disable();
    }
#endregion Init

#region Update
    void Update()
    {
        MoveCrosshairTwo();
        SelectTarget();
        CameraZoom();
       
    }

    private void MoveCrosshairOne()
    {
        //Vector2 mousePos = mouseAction.ReadValue<Vector2>();
        //Debug.Log(mousePos);

        // FIXME: Move the crosshair position to the mouse position (in world coordinates)
        Vector3 mousePosition = new Vector3(mouseAction.ReadValue<Vector2>().x, mouseAction.ReadValue<Vector2>().y, 0);
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);

        float distance = float.PositiveInfinity;
        RaycastHit hit;
        Vector3 hitpoint = Vector3.zero;
        if (Physics.Raycast(cameraRay, out hit, distance, wallsLayer))
        {
            distance = hit.distance;
            hitpoint = hit.point;
        }
        //Debug.Log(hitpoint);
        
        crosshair.position = hitpoint;
       
        //Crosshair is doing a weird thing where it will disapera under certain tiles
        //The crosshairs z postisiotn is not constant, does this effect it?

    }

    private void MoveCrosshairTwo()
    {
        Vector3 mousePosition = new Vector3(mouseAction.ReadValue<Vector2>().x, mouseAction.ReadValue<Vector2>().y, 0);
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);

        float distance;
        if (groundPlane.Raycast(cameraRay, out distance))
        {
            //Get the point that is clicked
            Vector3 hitPoint = cameraRay.GetPoint(distance);

            //Move your cube crosshar to the point where you clicked
            crosshair.position = hitPoint;
        }
    }


    private void SelectTarget()
    {
        if (selectAction.WasPerformedThisFrame())
        {
            // set the target position and invoke 
            target.gameObject.SetActive(true);
            target.position = crosshair.position;     
            TargetSelected?.Invoke(target.position);       
        }
    }

    private void CameraZoom()
    {
        float scroll = zoomAction.ReadValue<float>()/120f; 
        // Why is this value 120 or -120?
        //get it to 1 or -1 so it just has to know if its scrolling in or out.
        //like week 10 movement was an axis of -1 or 1.

        if(scroll != 0)
        {
            Debug.Log(scroll);

            //Use camera.main so much, just get a variable of it?
            if (Camera.main.orthographic) //false means its perspective
            {
                Camera.main.orthographicSize -= scroll;
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1f, 12f);
            }
            else
            {
                Debug.Log(scroll);
                Camera.main.fieldOfView -= scroll * 5f; //Smooth movement
                //suitable range, not too close and far enoguh that the whole level is in view.
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 20f, 100f);
            }
        }   
    }

#endregion Update

}
