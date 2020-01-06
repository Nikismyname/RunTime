using UnityEngine;

class DistanceTest : MonoBehaviour
{
    private void Start()
    {
        this.DistanceToHit(new Vector2(0, 0), new Vector3(1, 0), new Vector3(3, 1.2f)); //WORKS
        this.DistanceToHit(new Vector2(0, 0), new Vector3(-1, 0), new Vector3(3, 1.2f));
    }

    public DistanceToHitInformation DistanceToHit(Vector2 linePnt, Vector2 lineDir, Vector2 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        Vector2 v = pnt - linePnt;
        float d = Vector2.Dot(v, lineDir);
        Vector2 point = linePnt + lineDir * d;

        Ray ray = new Ray(linePnt, lineDir);

        Vector2 brp = linePnt;
        Vector2 brd = lineDir.Rotate(90);

        bool isToTheLeft = isPointLeftOfRay(point.x, point.y, brd.x, brd.y, brp.x, brp.y);

        if (isToTheLeft == false)
        {
            return null;
        }
        else
        {
            return new DistanceToHitInformation
            {
                HitVector = point - pnt,
                DistanceToProj = (point - linePnt).magnitude,
            };
        }

        bool isPointLeftOfRay(double x, double y, double raySx, double raySy, double rayEx, double rayEy)
        {
            return (y - raySy) * (rayEx - raySx)
            > (x - raySx) * (rayEy - raySy);
        }
    }
}
