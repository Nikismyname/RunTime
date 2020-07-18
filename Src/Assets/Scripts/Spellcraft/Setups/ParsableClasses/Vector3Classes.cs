using UnityEngine;

public class Vector3Classes
{
    public class Vector3Util
    {
        public Vector3 OffsetVector(Vector3 vec, float xOffset = 0, float yOffset = 0, float zOffset = 0)
        {
            Vector3 result = vec + new Vector3(xOffset, yOffset, zOffset);
            return result;
        }
    }
}

