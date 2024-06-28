using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;
    public Vector3 originPosition;
    private float inGameProjectionSize = 20.0f;
    private float GameIntervalProjectionSize = 50.0f;

    private float ProjectionProgressDuration = 0.7f;
    private float ProjectionCurrentProgress = 0.0f;

    private bool isProjectionToInterval = false;
    private bool isProjectionToStart = false;
    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isProjectionToInterval)
        {
            ProjectionToInterval();
        }
        else if (isProjectionToStart)
        {
            ProjectionToStart();
        }
        if (player != null)
        {
            transform.position = originPosition + (player.transform.position - new Vector3(0, 0, 0));
        }
        else {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void ProjectionToInterval() 
    {
        if (ProjectionCurrentProgress < ProjectionProgressDuration)
        {
            ProjectionCurrentProgress += Time.deltaTime;
            float t = ProjectionCurrentProgress / ProjectionProgressDuration;
            gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(inGameProjectionSize, GameIntervalProjectionSize, t);
        }
        else {
            ProjectionCurrentProgress = 0;
            isProjectionToInterval = false;
        }
    }

    void ProjectionToStart()
    {
        if (ProjectionCurrentProgress < ProjectionProgressDuration)
        {
            ProjectionCurrentProgress += Time.deltaTime;
            float t = ProjectionCurrentProgress / ProjectionProgressDuration;
            gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(GameIntervalProjectionSize, inGameProjectionSize, t);
        }
        else {
            ProjectionCurrentProgress = 0;
            isProjectionToStart = false;
        }
    }

    public void CallProjectionToInterval() {
        isProjectionToInterval = true;
    }

    public void CallProjectionToStart()
    {
        isProjectionToStart = true;
    }
}
