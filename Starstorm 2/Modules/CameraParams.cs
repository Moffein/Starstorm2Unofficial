using RoR2;
using UnityEngine;

namespace Starstorm2.Modules
{
    internal static class CameraParams
    {
        internal static CharacterCameraParams NewCameraParams(string name, Vector3 standardPosition)
        {
            return NewCameraParams(name, 70f, 1.37f, standardPosition, 0.1f);
        }

        internal static CharacterCameraParams NewCameraParams(string name, float pitch, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, 1.37f, standardPosition, 0.1f);
        }

        internal static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        internal static CharacterCameraParams NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition, float wallCushion)
        {
            CharacterCameraParams newParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newParams.maxPitch = pitch;
            newParams.minPitch = -pitch;
            newParams.pivotVerticalOffset = pivotVerticalOffset;
            newParams.standardLocalCameraPos = standardPosition;
            newParams.wallCushion = wallCushion;

            return newParams;
        }
    }
}