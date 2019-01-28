using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syy.Debug
{
    public static class DebugColliderViewer
    {
        public static Color Color = new Color(1, 0, 0, 0.25f);

        private static Dictionary<GameObject, IEnumerable<GameObject>> _viewerSet = new Dictionary<GameObject, IEnumerable<GameObject>>(8);

        public static void Show(GameObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException();
            }

            if (!Application.isPlaying)
            {
                throw new Exception("DebugColliderViewer is Playing only");
            }

            if (_viewerSet.ContainsKey(target))
            {
                Hide(target);
            }

            var viewers = new List<GameObject>(8);
            foreach (var collider in target.GetComponentsInChildren<Collider>())
            {
                var viewer = CreateColliderViewer(collider);
                viewers.Add(viewer);
            }
            _viewerSet[target] = viewers;
        }

        public static void Hide(GameObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException();
            }

            if (!Application.isPlaying)
            {
                throw new Exception("DebugColliderViewer is Playing only");
            }

            if (_viewerSet.ContainsKey(target))
            {
                var viewers = _viewerSet[target];
                foreach (var viewer in viewers)
                {
                    GameObject.DestroyImmediate(viewer);
                }
            }
        }

        private static GameObject CreateColliderViewer(Collider collider)
        {
            var viewer = default(GameObject);
            if (collider is BoxCollider)
            {
                viewer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var castCollider = (BoxCollider)collider;
                viewer.transform.localPosition = castCollider.center;
                viewer.transform.localScale = Vector3.Scale(viewer.transform.localScale, castCollider.size);
            }
            else if (collider is SphereCollider)
            {
                viewer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var castCollider = (SphereCollider)collider;
                viewer.transform.localPosition = castCollider.center;
                var max = Mathf.Max(castCollider.transform.localScale.x, castCollider.transform.localScale.y, castCollider.transform.localScale.z);
                var x = max / castCollider.transform.localScale.x;
                var y = max / castCollider.transform.localScale.y;
                var z = max / castCollider.transform.localScale.z;
                viewer.transform.localScale = new Vector3(x, y, z);
            }
            else if (collider is CapsuleCollider)
            {
                viewer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                var castCollider = (CapsuleCollider)collider;
                viewer.transform.localPosition = castCollider.center;

                switch (castCollider.direction)
                {
                    // X-Axis
                    case 0: viewer.transform.Rotate(Vector3.forward * 90f); break;
                    // Z-Axis
                    case 2: viewer.transform.Rotate(Vector3.right * 90f); break;
                }

                var x = viewer.transform.localScale.x * castCollider.radius * 2f;
                var y = viewer.transform.localScale.y * castCollider.height * 0.5f;
                var z = viewer.transform.localScale.z * castCollider.radius * 2f;
                viewer.transform.localScale = new Vector3(x, y, z);
            }
            else
            {
                throw new NotSupportedException("Not supported collider type. type=" + collider.GetType());
            }

            viewer.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DestroyImmediate(viewer.GetComponent<Collider>());
            viewer.transform.SetParent(collider.transform, false);

            var material = viewer.GetComponent<Renderer>().material;
            material.shader = Shader.Find("Sprites/Default");
            material.color = Color;

            return viewer;
        }
    }
}
