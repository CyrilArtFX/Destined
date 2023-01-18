using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Core
{
    [ExecuteInEditMode]
    [RequireComponent( typeof( MeshFilter ), typeof( MeshRenderer ) )]
    public class VisionFOV : MonoBehaviour
    {
        public MeshFilter Filter { get; private set; }
        public MeshRenderer Renderer { get; private set; }

        public LayerMask ObstaclesMask;
        public float FOV = 90.0f;
        public float ViewDistance = 1.0f;
        public int RayCount = 10;

        private Mesh mesh;

        void Awake()
        {
            mesh = new Mesh();

            Filter = GetComponent<MeshFilter>();
            Filter.mesh = mesh;

            Renderer = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            Refresh();
        }

        public void Refresh()
        {
            if ( RayCount <= 1 ) return;

            Vector3[] vertices = new Vector3[RayCount + 2];
            vertices[0] = Vector3.zero;

            int[] triangles = new int[RayCount * 3];
            Vector2[] uv = new Vector2[vertices.Length];

            int vertex_id = 1;
            int triangle_id = 0;
            float angle = FOV / 2.0f;
            float angle_step = FOV / RayCount;
            for ( int i = 0; i <= RayCount; i++ )
            {
                float rad_angle = Mathf.Deg2Rad * angle;
                Vector3 dir = new( Mathf.Cos( rad_angle ), Mathf.Sin( rad_angle ) );
                
                //  get vertex position
                Vector3 vertex;
                RaycastHit2D hit = Physics2DUtils.RaycastWithoutTrigger( transform.position, transform.TransformDirection( dir ), ViewDistance, ObstaclesMask );
                if ( hit )
                    vertex = transform.InverseTransformPoint( hit.point );
                else
                    vertex = dir * ViewDistance;

                //  assign vertex
                vertices[vertex_id] = vertex;

                //  assign triangle
                if ( i > 0 )
                {
                    triangles[triangle_id++] = 0;
                    triangles[triangle_id++] = vertex_id - 1;
                    triangles[triangle_id++] = vertex_id;
                }

                vertex_id++;
                angle -= angle_step;
            }

            if ( vertices.Length != mesh.vertices.Length )
                mesh.Clear();

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
	}
}
