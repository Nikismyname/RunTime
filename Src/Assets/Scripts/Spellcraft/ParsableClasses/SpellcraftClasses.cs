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
        private readonly float speed = 8f;
        private float timeSpan = 5f;
        public void Setup(Vector3 velocity)
        {
            this.velocity = velocity; 
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            this.timeSpan -= delta; 

            if(this.timeSpan <= 0)
            {
                Destroy(gameObject);
                Debug.Log("Destroyed!");
            }

            this.timeSpan -= delta;
            this.transform.position -= this.velocity * delta * this.speed;
        }
    }
}

