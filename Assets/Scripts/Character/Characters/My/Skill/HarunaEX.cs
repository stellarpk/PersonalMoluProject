using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class HarunaEX : MonoBehaviour
{
    public Character Owner;
    public MeshFilter myFilter;
    Vector3[] default_vb_Pos = new Vector3[5];
    Vector3[] cur_vb_Pos = new Vector3[5];
    public Transform Target;
    float[] dir = new float[4];
    private void Start()
    {
    }


    public IEnumerator DrawRange()
    {
        while (true)
        {
            Vector3[] vb = new Vector3[4];
            int[] ib = new int[6];
            vb[0] = new Vector3(0.1f, 0, 10.0f); // ����
            vb[1] = new Vector3(-0.1f, 0, 10.0f); // �»��
            vb[2] = new Vector3(0.1f, 0, 0); // ���ϴ�
            vb[3] = new Vector3(-0.1f, 0, 0); // ���ϴ�


            ib[0] = 0;
            ib[1] = 2;
            ib[2] = 1;

            ib[3] = 1;
            ib[4] = 2;
            ib[5] = 3;

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(0, 0);
            uv[3] = new Vector2(1, 0);

            Vector3[] normals = new Vector3[4];

            Vector3 n = Vector3.down;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = n;
            }

            Mesh _mesh = new Mesh();
            _mesh.vertices = vb;
            _mesh.triangles = ib;
            _mesh.uv = uv;
            _mesh.normals = normals;

            myFilter.mesh = _mesh;

            bool isColl = true;

            for (int i = 0; i < default_vb_Pos.Length - 1; i++)
            {
                default_vb_Pos[i] = vb[i] + transform.position;
            }
            default_vb_Pos[4] = new Vector3(0, 0, 5) + transform.position;

            for (int i = 0; i < cur_vb_Pos.Length; i++)
            {
                cur_vb_Pos[i] = transform.rotation * (default_vb_Pos[i] - transform.position) + transform.position;
            }

            Vector3[] targetUV = new Vector3[4];
            Vector3 min = Target.GetComponent<BoxCollider>().bounds.min;
            Vector3 max = Target.GetComponent<BoxCollider>().bounds.max;

            targetUV[0] = new Vector3(min.x, 0, min.z);
            targetUV[1] = new Vector3(max.x, 0, min.z);
            targetUV[2] = new Vector3(max.x, 0, max.z);
            targetUV[3] = new Vector3(min.x, 0, max.z);

            Vector3[] L = new Vector3[4];
            Vector3[] Center = new Vector3[2];
            Center[0] = Target.transform.position;
            Center[1] = cur_vb_Pos[4];
            Vector3 T = Center[1] - Center[0];

            L[0] = targetUV[3] - targetUV[0];
            L[1] = targetUV[1] - targetUV[0];
            L[2] = cur_vb_Pos[0] - cur_vb_Pos[2];
            L[3] = cur_vb_Pos[3] - cur_vb_Pos[2];

            for (int i = 0; i < L.Length; i++)
            {
                if (!CheckScan(T, L[i], L))
                {
                    isColl = false;
                    break;
                }
            }

            if (isColl)
            {
                Target.Rotate(Vector3.up * 360.0f * Time.deltaTime);
            }

            yield return null;
        }
    }

    bool CheckScan(Vector3 T ,Vector3 checkL, Vector3[] L)
    {
        float dis = Mathf.Abs(Vector3.Dot(T, checkL));
        float ra = Mathf.Abs(Vector3.Dot(checkL, L[0] * 0.5f)) + Mathf.Abs(Vector3.Dot(checkL, L[1] * 0.5f));
        float rb = Mathf.Abs(Vector3.Dot(checkL, L[2] * 0.5f)) + Mathf.Abs(Vector3.Dot(checkL, L[3] * 0.5f));
        if (dis > ra + rb) return false;
        else return true;
    }

    public IEnumerator RotateIndicator()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                Ray CamRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(CamRay, out hit))
                {
                    Vector3 mouseDir = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    transform.LookAt(mouseDir);
                }
            }
            yield return null;
        }
    }

}
