using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyExamples.RoundedRectangleGenerator
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode] 
    public class RoundedRectangleGenerator : Graphic
    {
        public float cornerRadius = 10f;
        public int cornerSegments = 8;

        private Vector2 lastSize;
        private float lastCornerRadius;
        private int lastCornerSegments;
        private Vector3 lastScale;

        protected override void Start()
        {
            base.Start();
            lastSize = rectTransform.rect.size;
            lastCornerRadius = cornerRadius;
            lastCornerSegments = cornerSegments;
            lastScale = rectTransform.localScale;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = rectTransform.rect;
            Vector2 size = rect.size;
            Vector3 scale = rectTransform.localScale;

            float scaledWidth = size.x * scale.x;
            float scaledHeight = size.y * scale.y;

            float maxRadius = Mathf.Min(scaledWidth, scaledHeight) / 2f;
            float actualRadius = Mathf.Min(cornerRadius, maxRadius);

            float localRadiusX = actualRadius / Mathf.Max(scale.x, 0.001f);
            float localRadiusY = actualRadius / Mathf.Max(scale.y, 0.001f);

            if (actualRadius <= 0 || cornerSegments < 1)
            {
                AddRectangleMesh(vh, rect, scale);
                return;
            }

            if (actualRadius >= maxRadius)
            {
                AddEllipseMesh(vh, rect, localRadiusX, localRadiusY);
                return;
            }

            CreateRoundedRectangleMesh(vh, rect, localRadiusX, localRadiusY, scale);
        }

        private void AddRectangleMesh(VertexHelper vh, Rect rect, Vector3 scale)
        {
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMin);
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMax);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMax);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMin);

            Vector2 uvScale = new Vector2(scale.x, scale.y);
            uvScale = new Vector2(
                Mathf.Approximately(uvScale.x, 0) ? 1 : Mathf.Abs(1f / uvScale.x),
                Mathf.Approximately(uvScale.y, 0) ? 1 : Mathf.Abs(1f / uvScale.y)
            );

            int startIndex = vh.currentVertCount;
            vh.AddVert(bottomLeft, color, new Vector2(0, 0));
            vh.AddVert(topLeft, color, new Vector2(0, 1));
            vh.AddVert(topRight, color, new Vector2(1, 1));
            vh.AddVert(bottomRight, color, new Vector2(1, 0));

            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }

        private void AddEllipseMesh(VertexHelper vh, Rect rect, float radiusX, float radiusY)
        {
            Vector3 center = rect.center;
            int totalVertices = cornerSegments * 4;

            int centerIndex = vh.currentVertCount;
            vh.AddVert(center, color, new Vector2(0.5f, 0.5f));

            for (int i = 0; i <= totalVertices; i++)
            {
                float angle = 2f * Mathf.PI * i / totalVertices;
                Vector3 pos = center + new Vector3(
                    Mathf.Cos(angle) * radiusX,
                    Mathf.Sin(angle) * radiusY,
                    0
                );

                Vector2 uv = new Vector2(
                    (pos.x - rect.xMin) / rect.width,
                    (pos.y - rect.yMin) / rect.height
                );

                vh.AddVert(pos, color, uv);
            }

            for (int i = 0; i < totalVertices; i++)
            {
                vh.AddTriangle(centerIndex, centerIndex + i + 1, centerIndex + i + 2);
            }
        }

        private void CreateRoundedRectangleMesh(VertexHelper vh, Rect rect, float radiusX, float radiusY, Vector3 scale)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            Vector3 center = rect.center;
            float halfWidth = rect.width / 2f;
            float halfHeight = rect.height / 2f;

            float innerWidth = rect.width - 2f * radiusX;
            float innerHeight = rect.height - 2f * radiusY;

            Vector3[] cornerCenters = {
            new Vector3(center.x + innerWidth/2, center.y + innerHeight/2, 0), // правый верхний
            new Vector3(center.x - innerWidth/2, center.y + innerHeight/2, 0), // левый верхний
            new Vector3(center.x - innerWidth/2, center.y - innerHeight/2, 0), // левый нижний
            new Vector3(center.x + innerWidth/2, center.y - innerHeight/2, 0)  // правый нижний
        };

            float[] startAngles = { 0, 90, 180, 270 };

            for (int corner = 0; corner < 4; corner++)
            {
                Vector3 cornerCenter = cornerCenters[corner];
                float startAngle = startAngles[corner];

                for (int i = 0; i <= cornerSegments; i++)
                {
                    float angle = Mathf.Deg2Rad * (startAngle + i * 90f / cornerSegments);
                    Vector3 pos = cornerCenter + new Vector3(
                        Mathf.Cos(angle) * radiusX,
                        Mathf.Sin(angle) * radiusY,
                        0
                    );

                    vertices.Add(pos);

                    Vector2 uv = new Vector2(
                        (pos.x - rect.xMin) / rect.width,
                        (pos.y - rect.yMin) / rect.height
                    );
                    uvs.Add(uv);
                }
            }

            int centerVertexIndex = vertices.Count;
            vertices.Add(center);
            uvs.Add(new Vector2(0.5f, 0.5f));

            int totalCornerVertices = (cornerSegments + 1) * 4;

            for (int i = 0; i < totalCornerVertices; i++)
            {
                int nextIndex = (i + 1) % totalCornerVertices;
                triangles.Add(centerVertexIndex);
                triangles.Add(i);
                triangles.Add(nextIndex);
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                vh.AddVert(vertices[i], color, uvs[i]);
            }

            for (int i = 0; i < triangles.Count; i += 3)
            {
                vh.AddTriangle(triangles[i], triangles[i + 1], triangles[i + 2]);
            }
        }

        void Update()
        {
            bool needsUpdate = false;

            Vector2 currentSize = rectTransform.rect.size;
            if (currentSize != lastSize)
            {
                lastSize = currentSize;
                needsUpdate = true;
            }

            if (cornerRadius != lastCornerRadius)
            {
                lastCornerRadius = cornerRadius;
                needsUpdate = true;
            }

            if (cornerSegments != lastCornerSegments)
            {
                lastCornerSegments = cornerSegments;
                needsUpdate = true;
            }

            Vector3 currentScale = rectTransform.localScale;
            if (currentScale != lastScale)
            {
                lastScale = currentScale;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                SetVerticesDirty();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            lastCornerRadius = cornerRadius;
            lastCornerSegments = cornerSegments;
            SetVerticesDirty();
        }
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            lastSize = rectTransform.rect.size;
            SetVerticesDirty();
        }
        public void ForceUpdate()
        {
            lastSize = rectTransform.rect.size;
            lastCornerRadius = cornerRadius;
            lastCornerSegments = cornerSegments;
            lastScale = rectTransform.localScale;
            SetVerticesDirty();
        }
    }
}