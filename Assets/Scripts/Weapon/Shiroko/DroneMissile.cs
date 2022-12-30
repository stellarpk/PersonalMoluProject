using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMissile : MonoBehaviour
{
    Vector3[] m_points = new Vector3[4];
    Rigidbody rigid;
    private float m_timerMax = 0;
    private float m_timerCur = 0;
    private float m_speed;
    public Transform target;
    public float damage;
    public bool isCritical;
    public GameObject Impact;
    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
    }
    public void Init(Transform _startTr, Transform _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr)
    {
        m_speed = _speed;

        m_timerMax = Random.Range(0.8f, 1.0f);

        m_points[0] = _startTr.position;
        m_points[1] = _startTr.position + (_newPointDistanceFromStartTr * Random.Range(-1.0f, 1.0f) * _startTr.right) +
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * _startTr.up)+
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, -0.8f) * _startTr.forward);
        m_points[2] = _endTr.position +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.right) + 
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.up) + 
            (_newPointDistanceFromEndTr * Random.Range(0.8f, 1.0f) * _endTr.forward);
        m_points[3] = _endTr.position;

        transform.position = _startTr.position;
    }

    private void Update()
    {
        if(m_timerCur > m_timerMax)
        {
            return;
        }
        m_timerCur += Time.deltaTime * m_speed;

        transform.position = new Vector3(
            CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, m_points[3].x),
            CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, m_points[3].y),
            CubicBezierCurve(m_points[0].z, m_points[1].z, m_points[2].z, m_points[3].z)
        );

        transform.LookAt(target);
    }

    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        float t = m_timerCur / m_timerMax;

        float ab = Mathf.Lerp(a, b, t);
        float bc = Mathf.Lerp(b, c, t);
        float cd = Mathf.Lerp(c, d, t);

        float abbc = Mathf.Lerp(ab, bc, t);
        float bccd = Mathf.Lerp(bc, cd, t);

        return Mathf.Lerp(abbc, bccd, t);
    }

    public void FinalDamage()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.gameObject.GetComponent<IBattle>() != null)
            {
                Instantiate(Impact, transform.position, transform.rotation);
                other.gameObject.GetComponent<IBattle>().OnDamage((int)damage, isCritical);
                Destroy(this.gameObject, 0.1f);
            }
            
        }
    }
}
