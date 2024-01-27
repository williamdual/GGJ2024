using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    public float surfaceArea;

    public List<Vector2> occupiedPositions = new List<Vector2>();
    // [SerializeField] public GameObject topLeft;
    // [SerializeField] public GameObject topRight;
    // [SerializeField] public GameObject bottomLeft;
    // [SerializeField] public Gameobject bottomRight;

    // public List<Vector2> GetFloorCorners()
    // {
    //     List<Vector2> corners = new List<Vector2>();
    //     corners.Add(topLeft.transform.position);
    //     corners.Add(topRight.transform.position);
    //     corners.Add(bottomLeft.transform.position);
    //     corners.Add(bottomRight.transform.position);
    //     return corners;
    // }

    void Start()
    {
        surfaceArea = transform.localScale.x * transform.localScale.y;
    }
    public bool FarEnoughApart(Vector2 newPosition, GameObject crowdMember)
    {
        if (occupiedPositions.Contains(newPosition))
        {
            return false;
        }
        float density = GetPopulationDensity();
        float tooClose = crowdMember.GetComponent<CrowdMember>().GetSize() / 4.0f;
        if (density > 0.1f)
        {
            return true;
        }
        Debug.Log($"Density: {density}");
        foreach (Vector2 position in occupiedPositions)
        {
            float distance = Mathf.Sqrt( Mathf.Pow(newPosition.x - position.x, 2) + Mathf.Pow(newPosition.y - position.y, 2));
            if (distance < tooClose)
            {
                Debug.Log($"Too close: {distance} < {tooClose}");
                Debug.Log($"Population density: {density}");
                return false;
            } 
        }
        return true;
    }


    public float GetPopulationDensity()
    {
        if (occupiedPositions.Count == 0)
        {
            return 0.01f;
        }
        return occupiedPositions.Count / surfaceArea;
    }

    public float GetSurfaceArea()
    {
        return surfaceArea;
    }   


}
