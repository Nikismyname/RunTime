using UnityEngine;

public class SpellcraftClasses
{
    public class Projectile
    {
        public void Shoot(Vector3 position, Vector3 velocity)
        {
            GameObject boolet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boolet.SetScale(new Vector3(0.3f,0.3f,0.3f));
            boolet.transform.position = position;
            BooletBehaviour beh = boolet.AddComponent<BooletBehaviour>();
            beh.Setup(velocity);
        }
    }

    public class BooletBehaviour: MonoBehaviour
    {
        private Vector3 velocity; 

        public void Setup(Vector3 velocity)
        {
            this.velocity = velocity; 
        }

        private void Update()
        {
            this.transform.position += this.velocity * Time.deltaTime;
        }
    }
}

