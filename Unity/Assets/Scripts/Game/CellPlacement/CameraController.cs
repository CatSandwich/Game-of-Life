using UnityEngine;

namespace CellPlacement
{
    public class CameraController : MonoBehaviour
    {
        public Camera Camera;
        public float Sensitivity;

        // Update is called once per frame
        void Update()
        {
            Camera.orthographicSize = Mathf.Clamp(Camera.orthographicSize - Input.mouseScrollDelta.y, 0.1f, float.PositiveInfinity);
            transform.Translate(
                new Vector3(
                    Input.GetAxis("Horizontal") * transform.localScale.x * Sensitivity * Camera.orthographicSize, 
                    Input.GetAxis("Vertical") * transform.localScale.y * Sensitivity * Camera.orthographicSize, 
                    0
                )
            );
        }
    }
}
