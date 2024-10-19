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

    public LayerMask Walls;
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

        Cursor.visible = false;
        target.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        actions.mouse.Enable();
    }

    void OnDisable()
    {
        actions.mouse.Disable();
    }
#endregion Init

#region Update
    void Update()
    {
        MoveCrosshair();
        SelectTarget();

       
    }

    private void MoveCrosshair()
    {
        //Vector2 mousePos = mouseAction.ReadValue<Vector2>();
        //Debug.Log(mousePos);

        // FIXME: Move the crosshair position to the mouse position (in world coordinates)
        Vector3 mousePosition = new Vector3(mouseAction.ReadValue<Vector2>().x, mouseAction.ReadValue<Vector2>().y, 0);
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);

        float distance = float.PositiveInfinity;
        RaycastHit hit;
        Vector3 hitpoint = Vector3.zero;
        if (Physics.Raycast(cameraRay, out hit, distance, Walls))
        {
            distance = hit.distance;
            hitpoint = hit.point;
        }
        //Debug.Log(hitpoint);
        
        crosshair.position = hitpoint;
       
        //Crosshair is doing a weird thing where it will disapera under certain tiles
        //The crosshairs z postisiotn is not constant, does this effect it?

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

#endregion Update

}
