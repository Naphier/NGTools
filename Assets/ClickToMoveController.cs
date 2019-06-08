using UnityEngine;
using System.Collections;

public class ClickToMoveController : MonoBehaviour
{
    public string MoveableObjectTag = "Player";
    public string GroundTag = "Finish";
    public float Speed = 10f;
    public bool restrictX = false;
    public bool restrictY = true;
    public bool restrictZ = false;


    private GameObject selected = null;
    private Vector3? waypoint = null; 

    void Update()
    {
        Execute();
    }

    public void Execute()
    {
        RaycastHit hit = SelectObjectToMove();
        if (hit.collider && hit.collider.gameObject)
        {
            selected = hit.collider.gameObject;
            waypoint = null;
        }

        if (selected)
        {
            RaycastHit hit2 = SelectLocation();
            if (hit2.collider)
                waypoint = hit2.point;
        }

        // No, should really be an array of commands.
        MoveToWaypoint(ref waypoint, selected);
    }

    public RaycastHit SelectObjectToMove()
    {
        RaycastHit hitInfo = new RaycastHit();

        if (Input.GetMouseButtonDown(0))
        {
            Ray pointer = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(pointer, out hitInfo))
            {
                if (hitInfo.collider.tag == MoveableObjectTag)
                    Debug.Log("Selected: " + hitInfo.collider.name);
                else
                    hitInfo = new RaycastHit();
            }

            Debug.DrawRay(pointer.origin, pointer.direction, Color.magenta, 2f);
        }

        return hitInfo;
    }

    public RaycastHit SelectLocation()
    {
        RaycastHit hitInfo = new RaycastHit();

        if (Input.GetMouseButtonDown(0))
        {
            Ray pointer = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(pointer, out hitInfo))
            {
                if (hitInfo.collider.tag == GroundTag)
                    Debug.Log("Pointed to: " + hitInfo.collider.name);
                else
                    hitInfo = new RaycastHit();
            }

            Debug.DrawRay(pointer.origin, pointer.direction, Color.magenta, 2f);
        }

        return hitInfo;
    }

    public void MoveToWaypoint(ref Vector3? waypoint, GameObject toMove)
    {
        if (waypoint == null || toMove == null)
            return;

        Vector3 position = toMove.transform.position;
        Vector3 final = (Vector3)waypoint;

        if (restrictX)
            final.x = toMove.transform.position.x;

        if (restrictY)
            final.y = toMove.transform.position.y;

        if (restrictZ)
            final.z = toMove.transform.position.z;

        position = Vector3.Lerp(toMove.transform.position, final, Time.deltaTime * Speed);

        if (position == final)
            waypoint = null;
        else
            toMove.transform.position = position;

    }



}
