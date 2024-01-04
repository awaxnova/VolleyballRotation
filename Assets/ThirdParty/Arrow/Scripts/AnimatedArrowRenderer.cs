using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Arrow
{
    public class AnimatedArrowRenderer : ArrowRenderer
    {
        [Space] [SerializeField] float fadeDistance = 0.35f;
        [SerializeField] float speed = 1f;
        [SerializeField] List<GameObject> tipPrefabs;
        [SerializeField] List<GameObject> segmentPrefabs;
        
        public enum ArrowTypes { RedPoint, GreenPoint, BlueZip, None, };
        public enum SegmentTypes { RedRect, GreenTube, BlueZip, None, };

        private ArrowTypes selectedArrowType = ArrowTypes.RedPoint;
        private SegmentTypes selectedSegmentType = SegmentTypes.RedRect;

        private ArrowTypes nextSelectedArrowType = ArrowTypes.RedPoint;
        private SegmentTypes nextSelectedSegmentType = SegmentTypes.RedRect;

        // Getter for the tip prefab
        GameObject tipPrefab => tipPrefabs[(int)selectedArrowType];

        // Getter for the segment prefab
        GameObject segmentPrefab => segmentPrefabs[(int)selectedSegmentType];

        Transform arrow;

        readonly List<Transform> segments = new();
        readonly List<MeshRenderer> renderers = new();

        protected override float Offset => Time.time * speed;
        protected override float FadeDistance => fadeDistance;

        private bool arrowEnabled = true;

        private bool shadowsEnabled = false;

        protected override void Update()
        {
            base.Update();

            if ((!arrowEnabled) ||(end == start))
            {
                TurnOffArrow();
            }
            else
            {
                var shadowMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                if (shadowsEnabled)
                    shadowMode = UnityEngine.Rendering.ShadowCastingMode.On;

                foreach (var rend in renderers)
                {
                        rend.shadowCastingMode = shadowMode;
                }

                if (arrow != null)
                {
                    arrow.GetComponent<MeshRenderer>().shadowCastingMode = shadowMode;
                }

                UpdateSegments();
            }
        }

        public void TurnOnArrow()
        {
            arrowEnabled = true;
        
            if (arrow)
                arrow.gameObject.SetActive(true);
        }

        public void TurnOffArrow()
        {
            arrowEnabled = false;

            if (arrow)
                arrow.gameObject.SetActive(false);

            foreach (var segment in segments)
                segment.gameObject.SetActive(false);
        }

        void UpdateSegments()
        {
            Debug.DrawLine(start, end, Color.yellow);
                        
            UpdateArrowSegment(nextSelectedSegmentType, Positions.Count - 1);

            for (var i = 0; i < Positions.Count - 1; i++)
            {
                segments[i].localPosition = Positions[i];
                segments[i].localRotation = Rotations[i];

                var meshRenderer = renderers[i];

                if (!meshRenderer)
                    continue;

                var material = meshRenderer.material;

                var currentColor = material.color;
                currentColor.a = Alphas[i];
                material.color = currentColor;
            }

            UpdateArrowHead(nextSelectedArrowType);

            transform.position = start;
            transform.rotation = Quaternion.LookRotation(end - start, upwards);
        }

        void UpdateArrowHead(ArrowTypes newArrowHeadType)
        { 

            // If the selected arrowhead matches the currently instantiated arrowhead, leave it in place,
            if(newArrowHeadType != selectedArrowType)
            {
                // but if the arrow isn't the same,
                // destroy the existing one if it exists,
                if (arrow)
                {
                    Destroy(arrow.gameObject);
                }

                // choose the new arrow type
                selectedArrowType = newArrowHeadType;
            }

            // If the arrow isn't instantiated, then instantiate it,
            if(!arrow)
            {
                arrow = Instantiate(tipPrefab, transform).transform;
            }

            // and setup the new position and rotation.
            arrow.localPosition = Positions.Last();
            arrow.localRotation = Rotations.Last();
        }

        void UpdateArrowSegment(SegmentTypes newArrowSegmentType, int segmentsCount)
        {
            // If the segment is already the selected segment, then leave it in place.
            if(newArrowSegmentType != selectedSegmentType)
            {
                // Otherwise,
                // clear the existing renderers, and
                // destroy the existing segments
                // and clear the segments list.

                renderers.Clear();

                foreach (var segment in segments)
                {
                    Destroy(segment.gameObject);
                }

                segments.Clear();

                selectedSegmentType = newArrowSegmentType;
            }

            // If the segments list is too short, then instantiate the remaining segments, and add the renderers to the renderers list.

            CheckSegments(segmentsCount);
        }

        void CheckSegments(int segmentsCount)
        {
            while (segments.Count < segmentsCount)
            {
                var segment = Instantiate(segmentPrefab, transform).transform;
                segments.Add(segment);
                renderers.Add(segment.GetComponent<MeshRenderer>());
            }

            for (var i = 0; i < segments.Count; i++)
            {
                var segment = segments[i].gameObject;
                if (segment.activeSelf != i < segmentsCount)
                    segment.SetActive(i < segmentsCount);
            }
        }

        public void SetHeight(float height)
        {
            this.height = height;
        }

        public void SetSegmentLength(float segmentLength)
        {
            this.segmentLength = segmentLength;
        }

        public void SetShadowMode(bool enableCastShadow)
        {
            shadowsEnabled = enableCastShadow;
        }

        public void SetArrowHeadType(ArrowTypes arrowHeadType)
        {
            nextSelectedArrowType = arrowHeadType;
            UpdateArrowHead(arrowHeadType);
        }

        public void SetArrowSegmentType(SegmentTypes arrowSegmentType)
        {
            nextSelectedSegmentType = arrowSegmentType;
            UpdateArrowSegment(arrowSegmentType, Positions.Count - 1);
        }
    }
}