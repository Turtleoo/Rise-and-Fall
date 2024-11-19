using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject cameraSubject;
    // Camera's position will be the same as the position of the car
    void LateUpdate()
    {
        transform.position = cameraSubject.transform.position + new Vector3(0,0,-5);
    }
}
