using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering;
using System;

namespace WaterSystem
{
    /// <summary>
    /// Custom HDRP render pass for water-specific rendering effects.
    /// Handles planar reflections, refractions, and underwater screen effects.
    /// </summary>
    [Serializable]
    public class WaterCustomPass : CustomPass
    {
        [Header("Pass Configuration")]
        public WaterSystem targetWaterSystem;
        
        [Header("Reflection Settings")]
        public bool enablePlanarReflections = true;
        public LayerMask reflectionLayers = -1;
        [Range(0.1f, 4f)]
        public float reflectionResolutionScale = 0.5f;
        
        [Header("Refraction Settings")]
        public bool enableRefraction = true;
        [Range(0f, 1f)]
        public float refractionStrength = 0.5f;

        // Render textures
        private RTHandle planarReflectionRT;
        private RTHandle refractionRT;
        
        // Materials
        private Material waterMaterial;
        private Material blitMaterial;

        // Property IDs
        private static readonly int ReflectionTextureID = Shader.PropertyToID("_PlanarReflectionTexture");
        private static readonly int RefractionTextureID = Shader.PropertyToID("_RefractionTexture");
        private static readonly int RefractionStrengthID = Shader.PropertyToID("_RefractionStrength");

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            // Allocate render textures
            AllocateReflectionRT();
            AllocateRefractionRT();
            
            // Find water material
            if (targetWaterSystem != null)
            {
                MeshRenderer renderer = targetWaterSystem.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    waterMaterial = renderer.sharedMaterial;
                }
            }
        }

        protected override void Execute(CustomPassContext ctx)
        {
            if (targetWaterSystem == null || !targetWaterSystem.enabled)
                return;

            // Render planar reflections
            if (enablePlanarReflections && planarReflectionRT != null)
            {
                RenderPlanarReflection(ctx);
            }

            // Render refraction
            if (enableRefraction && refractionRT != null)
            {
                RenderRefraction(ctx);
            }

            // Bind textures to water material
            BindTexturesToMaterial(ctx.cmd);
        }

        private void RenderPlanarReflection(CustomPassContext ctx)
        {
            // Get reflection camera
            Camera mainCamera = ctx.hdCamera.camera;
            
            // Create reflection matrix
            Vector3 waterNormal = Vector3.up;
            float waterHeight = targetWaterSystem.transform.position.y;
            Vector4 reflectionPlane = new Vector4(waterNormal.x, waterNormal.y, waterNormal.z, -waterHeight);
            
            Matrix4x4 reflectionMatrix = CalculateReflectionMatrix(reflectionPlane);
            
            // Setup reflection camera position
            Vector3 originalPos = mainCamera.transform.position;
            Vector3 reflectedPos = reflectionMatrix.MultiplyPoint(originalPos);
            
            // Configure culling
            ctx.cmd.SetInvertCulling(true);
            
            // Note: Full implementation would render scene from reflected viewpoint
            // This is a simplified version - full implementation in later stages
            
            ctx.cmd.SetInvertCulling(false);
            
            // Set reflection texture on water material
            if (waterMaterial != null)
            {
                ctx.cmd.SetGlobalTexture(ReflectionTextureID, planarReflectionRT);
            }
        }

        private void RenderRefraction(CustomPassContext ctx)
        {
            // Copy current color buffer for refraction
            HDUtils.BlitCameraTexture(ctx.cmd, ctx.cameraColorBuffer, refractionRT);
            
            // Set refraction parameters
            ctx.cmd.SetGlobalTexture(RefractionTextureID, refractionRT);
            ctx.cmd.SetGlobalFloat(RefractionStrengthID, refractionStrength);
        }

        private void BindTexturesToMaterial(CommandBuffer cmd)
        {
            if (waterMaterial == null)
                return;

            if (enablePlanarReflections && planarReflectionRT != null)
            {
                cmd.SetGlobalTexture(ReflectionTextureID, planarReflectionRT);
            }

            if (enableRefraction && refractionRT != null)
            {
                cmd.SetGlobalTexture(RefractionTextureID, refractionRT);
                cmd.SetGlobalFloat(RefractionStrengthID, refractionStrength);
            }
        }

        private Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
        {
            Matrix4x4 reflectionMat = Matrix4x4.identity;

            reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
            reflectionMat.m01 = -2f * plane[0] * plane[1];
            reflectionMat.m02 = -2f * plane[0] * plane[2];
            reflectionMat.m03 = -2f * plane[3] * plane[0];

            reflectionMat.m10 = -2f * plane[1] * plane[0];
            reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
            reflectionMat.m12 = -2f * plane[1] * plane[2];
            reflectionMat.m13 = -2f * plane[3] * plane[1];

            reflectionMat.m20 = -2f * plane[2] * plane[0];
            reflectionMat.m21 = -2f * plane[2] * plane[1];
            reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
            reflectionMat.m23 = -2f * plane[3] * plane[2];

            reflectionMat.m30 = 0f;
            reflectionMat.m31 = 0f;
            reflectionMat.m32 = 0f;
            reflectionMat.m33 = 1f;

            return reflectionMat;
        }

        private void AllocateReflectionRT()
        {
            if (planarReflectionRT != null)
                return;

            planarReflectionRT = RTHandles.Alloc(
                scaleFactor: Vector2.one * reflectionResolutionScale,
                colorFormat: GraphicsFormat.R16G16B16A16_SFloat,
                dimension: TextureDimension.Tex2D,
                enableRandomWrite: false,
                useMipMap: false,
                autoGenerateMips: false,
                name: "Planar Reflection RT"
            );
        }

        private void AllocateRefractionRT()
        {
            if (refractionRT != null)
                return;

            refractionRT = RTHandles.Alloc(
                scaleFactor: Vector2.one,
                colorFormat: GraphicsFormat.R16G16B16A16_SFloat,
                dimension: TextureDimension.Tex2D,
                enableRandomWrite: false,
                useMipMap: false,
                autoGenerateMips: false,
                name: "Refraction RT"
            );
        }

        protected override void Cleanup()
        {
            planarReflectionRT?.Release();
            refractionRT?.Release();
        }
    }
}