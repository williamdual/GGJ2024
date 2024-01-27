using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{

    private Vector2 boundsOffset = new Vector2(0.3f, 0.3f);
    [SerializeField] private GameObject crowdMemberPrefab; 
    [SerializeField] private GameObject floorPrefab;

    private GameObject floor;

    [SerializeField] private Vector2 topLeft;
    [SerializeField] private Vector2 bottomRight;


    //Debug Function
    
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         OnClick();
    //     }
    // }

    public GameObject SpawnCrowdMember()
    {
        GameObject crowdMember = Instantiate(crowdMemberPrefab, transform.position, Quaternion.identity);
        crowdMember.transform.parent = transform;
        return crowdMember;
    }

    public GameObject SpawnFloor(float xOffset, float yOffset)
    {
        floor = Instantiate(floorPrefab, new Vector2(transform.position.x + xOffset, transform.position.y + yOffset), Quaternion.identity);
        floor.transform.parent = transform;
        return floor;
    }


    private void ChooseRandomPosition(GameObject crowdMember)
    {
        float x = Random.Range(topLeft.x + boundsOffset.x, bottomRight.x - boundsOffset.x);
        float y = Random.Range(topLeft.y + boundsOffset.y, bottomRight.y - boundsOffset.y);
        while (!floor.GetComponent<Floor>().FarEnoughApart(new Vector2(x, y), crowdMember))
        {
            x = Random.Range(topLeft.x + boundsOffset.x, bottomRight.x - boundsOffset.x);
            y = Random.Range(topLeft.y + boundsOffset.y, bottomRight.y - boundsOffset.y);
        }
        floor.GetComponent<Floor>().occupiedPositions.Add(new Vector2(x, y));
        crowdMember.GetComponent<CrowdMember>().targetPosition = new Vector2(x, y);
    }

    void Start()
    {
        floor = SpawnFloor(0, -1);
        topLeft = floor.GetComponent<Renderer>().bounds.min;
        bottomRight = floor.GetComponent<Renderer>().bounds.max;
    }
    //Debug Function


    // public void OnClick()
    // {  
    //     GameObject crowdMember = SpawnCrowdMember();   
    //     crowdMember.GetComponent<CrowdMember>().GetSize();
    //     ChooseRandomPosition(crowdMember);
    //     // Debug.Log($" Crowd Members: {floor.GetComponent<Floor>().occupiedPositions.Count}");
    //     // Debug.Log($" Population Density: {floor.GetComponent<Floor>().GetPopulationDensity()}");
    // }
}