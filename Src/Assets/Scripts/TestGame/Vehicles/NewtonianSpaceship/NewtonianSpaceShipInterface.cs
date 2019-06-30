using UnityEngine;

public class NewtonianSpaceShipInterface : MonoBehaviour
{
    public NewtonianSpaceshipHandling handling;

    public void Drive()
    {
        this.handling.Drive();
    }

    public void Exit()
    {
        this.handling.Exit();
    }
}
