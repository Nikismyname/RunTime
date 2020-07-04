using UnityEngine;

public class NewtonianSpaceShipInterface : MonoBehaviour
{
    public NewtonianSpaceshipHandling handling;
    private TrackManager trackManager;

    private void Awake()
    {
        this.trackManager = new TrackManager(gameObject);
    }

    public void Drive()
    {
        this.handling.Drive();
    }

    public void Exit()
    {
        this.handling.Exit();
    }

    public void StartRace()
    {
        this.handling.Drive();
        this.trackManager.StartRace();
    }
}
