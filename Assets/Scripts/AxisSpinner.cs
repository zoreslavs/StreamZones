using UnityEngine;

public class AxisSpinner : MonoBehaviour
{
    [SerializeField] private float _speed = 100f;
    [SerializeField] private bool _axisX;
    [SerializeField] private bool _axisY;
    [SerializeField] private bool _axisZ;

    private void Update()
    {
        float angle = _speed * Time.deltaTime;
        transform.Rotate(
            _axisX ? angle : 0f,
            _axisY ? angle : 0f,
            _axisZ ? angle : 0f
        );
    }
}