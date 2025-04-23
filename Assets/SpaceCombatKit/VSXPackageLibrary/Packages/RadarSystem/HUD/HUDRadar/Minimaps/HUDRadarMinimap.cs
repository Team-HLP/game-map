using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VSX.RadarSystem
{

    /// <summary>
    /// Displays a minimap to go along with a HUD Radar 2D. Can display a realtime minimap camera render, or a pre-baked map texture.
    /// </summary>
    public class HUDRadarMinimap : MonoBehaviour
    {

        [Tooltip("The HUD radar that this minimap is displayed with.")]
        [SerializeField] protected HUDRadar HUDRadar;


        [Tooltip("The Rect Transform for the map Image or Raw Image.")]
        [SerializeField] protected RectTransform map;


        [Tooltip("The RectTransform for the map pivot (rotation handle). Must be a parent of the map.")]
        [SerializeField] protected RectTransform mapPivot;



        [Header("Minimap Camera")]


        [Tooltip("The camera that renders the overhead minimap - not required for baked map.")]
        [SerializeField] protected Camera minimapCamera;

        protected RenderTexture m_RenderTexture;


        [Tooltip("The resolution of the render texture that the minimap camera renders into.")]
        [SerializeField] int renderTextureSize = 1024;


        [Tooltip("The Raw Image used to display the rendered minimap on the UI.")]
        [SerializeField] RawImage renderTextureDisplay;


        [Header("Baked Map")]


        [Tooltip("The size (side length) of the area represented by the baked map, in world units.")]
        [SerializeField]
        protected float bakedMapWorldSize = 1000;



        protected virtual void Reset()
        {
            HUDRadar = GetComponent<HUDRadar>();

            minimapCamera = GetComponentInChildren<Camera>();
        }


        protected virtual void Awake()
        {
            // Prepare the minimap camera
            if (minimapCamera != null)
            {
                m_RenderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 24);
                minimapCamera.targetTexture = m_RenderTexture;
                renderTextureDisplay.texture = m_RenderTexture;
            }
        }


        protected virtual void LateUpdate()
        {
            if (HUDRadar.Trackers.Count == 0) return;

            if (minimapCamera != null)
            {
                minimapCamera.transform.position = new Vector3(HUDRadar.Trackers[0].transform.position.x, minimapCamera.transform.position.y, HUDRadar.Trackers[0].transform.position.z);
                minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

                minimapCamera.orthographicSize = HUDRadar.DisplayRange;
            }
            else
            {
                Vector3 mapPos = -HUDRadar.Trackers[0].transform.position.normalized * (HUDRadar.Trackers[0].transform.position.magnitude / bakedMapWorldSize) * (map.sizeDelta.x / 2);
                map.anchoredPosition = new Vector2(mapPos.x, mapPos.z);

                float nextMapSize = (bakedMapWorldSize / HUDRadar.DisplayRange) * HUDRadar.EquatorRadius * 2;
                map.sizeDelta = new Vector2(nextMapSize, nextMapSize);
            }

            Vector3 refVector = HUDRadar.Trackers[0].ReferenceTransform.forward;
            refVector.y = 0;
            float ang = Vector3.Angle(Vector3.forward, refVector);
            mapPivot.localRotation = Quaternion.Euler(0f, 0f, ang * Mathf.Sign(HUDRadar.Trackers[0].ReferenceTransform.forward.x));
        }
    }
}
