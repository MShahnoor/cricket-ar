using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    [Tooltip("This will be instantiated on touch")]
    [SerializeField]
    GameObject arObjectToSpawn;

    [SerializeField]
    [Tooltip("This will be indicator to place object")]
    GameObject placementIndicator;

    [SerializeField]
    [Tooltip("Distance from the camera to place the object.")]
    float placementDistance = 1;

    // Reference to ARPlaneManager
    private ARPlaneManager arPlaneManager;  

    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    void Start()
    {
        aRRaycastManager = FindFirstObjectByType<ARRaycastManager>();

        // Find ARPlaneManager in the scene
        arPlaneManager = FindFirstObjectByType<ARPlaneManager>();  
    }

    void Update()
    {
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }

        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    void UpdatePlacementPose()
    {
        // Calculate the screen center, (0.5f, 0.5f) represents the center along x and y axis
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        // List to store raycast hits
        var hits = new List<ARRaycastHit>();

        // Perform a raycast using ARRaycastManager against detected planes
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        // Update placementPoseIsValid based on whether any planes were hit
        placementPoseIsValid = hits.Count > 0;

        // If valid, set PlacementPose to the pose of the first hit plane
        if (placementPoseIsValid)
        {
            // as the UpdatePlacementPose is called each frame, PlacementPose contains position of latest detected plane
            PlacementPose.rotation = hits[0].pose.rotation;

            // Place object at specific distance from camera. distance is specified in placementDistance variable
            PlacementPose.position = Camera.main.transform.position + Camera.main.transform.forward * placementDistance;

            // vertically the PlacementPosition should match the plane
            PlacementPose.position.y = hits[0].pose.position.y;
        }
    }

    void UpdatePlacementIndicator()
    {
        /*
         * Show and update plaement indicator if:
         *  1. object hasn't been already spawned
         *  2. A place has been detected (placementPoseIsValid will be only when there's a plane detected)
         */
        if (spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void ARPlaceObject()
    {
        // Object will be spawned where latest raycast was hit (PlacementPose)
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);

        // Deactivate or destroy the AR Plane Manager
        if (arPlaneManager != null)
        {
            foreach (var planes in arPlaneManager.trackables)
            {
                planes.gameObject.SetActive(false);
            }

            arPlaneManager.enabled = false;
        }
    }
}
