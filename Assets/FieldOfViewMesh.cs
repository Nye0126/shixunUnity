using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewMesh : MonoBehaviour
{
    private Newenemies fov; 
    private Mesh mesh;
    public int rayCount = 50; // 射线数量，越多越圆滑
    public Color normalColor = new Color(1f, 0.92f, 0.016f, 0.3f); // 半透明黄
    public Color alertColor = new Color(1f, 0f, 0f, 0.5f);        // 半透明红
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = GetComponentInParent<Newenemies>();
        meshRenderer = GetComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        DrawFOV();
        meshRenderer.material.color = fov.IsAlerted ? alertColor : normalColor;

    }
    void DrawFOV()
    {
    
        float angle = fov.viewAngle;
        float radius = fov.viewRadius;

        int vertexCount = rayCount + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // 原点

        for (int i = 0; i <= rayCount; i++)
        {
            // 计算当前射线的角度
            float currentAngle = -(angle / 2) + (angle / rayCount) * i;
            Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;

        
            RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, transform.parent.TransformDirection(dir), radius, fov.obstacleMask);

            if (hit.collider == null)
            {
                vertices[i + 1] = dir * radius;
            }
            else
            {
                // 如果撞到墙，网格就缩回到撞击点位置
                vertices[i + 1] = transform.parent.InverseTransformPoint(hit.point);
            }

            if (i < rayCount)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}

