using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that handles the visuals of movement and attacking. 
public class UnitView : MonoBehaviour
{
    const float rotationSpeed = 180f;
    const float travelSpeed = 4f;

    List<HexCell> pathToTravel;

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    float orientation;

    HexCell currentTravelLocation;

    public Unit unit;

    private void OnEnable()
    {
        unit = GetComponent<Unit>();
        unit.OnUnitMoved += OnUnitMoved;

        if (currentTravelLocation)
        {
            unit.Grid.DecreaseVisibility(currentTravelLocation, unit.visionRange);
            currentTravelLocation = null;
        }
    }

    public void OnUnitMoved(List<HexCell> path)
    {
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }


    IEnumerator TravelPath()
    {
        ClearPath();
        Debug.Log("path should not be showing until after the coroutine.");

        Vector3 a, b, c = pathToTravel[0].Position;
        yield return LookAt(pathToTravel[1].Position);

        if (!currentTravelLocation)
        {
            currentTravelLocation = pathToTravel[0];
        }
        unit.Grid.DecreaseVisibility(currentTravelLocation, unit.visionRange);
        int currentColumn = currentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            currentTravelLocation = pathToTravel[i];
            a = c;
            b = pathToTravel[i - 1].Position;

            int nextColumn = currentTravelLocation.ColumnIndex;
            if (currentColumn != nextColumn)
            {
                if (nextColumn < currentColumn - 1)
                {
                    a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                }
                else if (nextColumn > currentColumn + 1)
                {
                    a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                }
                unit.Grid.MakeChildOfColumn(transform, nextColumn);
                currentColumn = nextColumn;
            }

            c = (b + currentTravelLocation.Position) * 0.5f;
            unit.Grid.IncreaseVisibility(pathToTravel[i], unit.visionRange);

            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            unit.Grid.DecreaseVisibility(pathToTravel[i], unit.visionRange);
            t -= 1f;
        }
        currentTravelLocation = null;

        a = c;
        b = unit.Location.Position;
        c = b;
        unit.Grid.IncreaseVisibility(unit.Location, unit.visionRange);
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = unit.Location.Position;
        orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
    }

    IEnumerator LookAt(Vector3 point)
    {
        if (HexMetrics.Wrapping)
        {
            float xDistance = point.x - transform.localPosition.x;
            if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
        }

        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);

        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime * speed
            )
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        transform.LookAt(point);
        orientation = transform.localRotation.eulerAngles.y;
    }

    public void ShowPath()
    {
        List<HexCell> path = unit.GetCurrentPath();
        if (path != null && path.Count > 1)
        {
            Debug.Log("Showing unit path.");
            foreach (HexCell cell in path)
            {
                int turn = (cell.MovementCost - 1) / unit.movement;

                if (unit.movementRemaining == 0)
                {
                    turn++;
                }
                else if (unit.movementRemaining < unit.movement)
                {
                    int remainder = (cell.MovementCost - 1) % unit.movement;
                    if (remainder >= unit.movementRemaining)
                    {
                        turn++;
                    }
                }

                cell.SetLabel(turn.ToString());
                cell.EnableHighlight(Color.white);
            }

            path[0].EnableHighlight(Color.blue);
            path[0].SetLabel(null);
            path[path.Count - 1].EnableHighlight(Color.red);
        }
    }

    public void ClearPath()
    {
        List<HexCell> path = unit.GetCurrentPath();
        if (path != null && path.Count >= 1)
        {
            Debug.Log("Clearing old unit path.");
            foreach (HexCell cell in path)
            {
                cell.SetLabel(null);
                cell.DisableHighlight();
            }

            path[path.Count - 1].DisableHighlight();
        }
    }
}
