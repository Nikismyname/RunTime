using System.Threading.Tasks;
using UnityEngine;

public class TurretInt : MonoBehaviour
{
    private TurretLevel level;

    public void Setup(TurretLevel level)
    {
        this.level = level;
    }

    public async void ShootLoop()
    {
        while (this != null)
        {
            for (int i = 0; i < this.level.targets.Count; i++)
            {
                
                if (this != null && this.level.targets[i] == null)
                {
                    await Task.Delay(300);
                    continue;
                }
                else
                {
                    if (this != null)
                    {
                        this.level.ShootBoolet(new GenericBoolet
                        {
                            Body = null,
                            InitialPosition = this.gameObject.transform.position,
                            Velocity = this.CalculateVelocity(this.level.targets[i].Body)
                        });
                    }

                    await Task.Delay(300);
                }
            }
        }
    }

    private Vector3 CalculateVelocity(GameObject target)
    {
        return new Vector3(0, 1, 0);
    }

    public static string Source = @"
using System.Threading.Tasks;
using UnityEngine;

public class TurretInt : MonoBehaviour
{
    private TurretLevel level;

    public void Setup(TurretLevel level)
    {
        this.level = level;
    }

    public async void ShootLoop()
    {
        while (this != null)
        {
            for (int i = 0; i < this.level.targets.Count; i++)
            {
                
                if (this != null && this.level.targets[i] == null)
                {
                    await Task.Delay(300);
                    continue;
                }
                else
                {
                    if (this != null)
                    {
                        this.level.ShootBoolet(new GenericBoolet
                        {
                            Body = null,
                            InitialPosition = this.gameObject.transform.position,
                            Velocity = this.CalculateVelocity(this.level.targets[i].Body)
                        });
                    }

                    await Task.Delay(300);
                }
            }
        }
    }

    private Vector3 CalculateVelocity(GameObject target)
    {
        return new Vector3(0, 1, 0);
    }
}
";
}
