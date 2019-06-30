#region INIT
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetBehaviour : MonoBehaviour, IPointerDownHandler
{
    public int id;
    private Main ms;
    private bool selected = false;
    private bool flash = false;
    private Renderer myRenderer;
    private Color originalColor;
    private Color selectionColor = Color.green;
    private List<CancellationTokenSource> cancelationSources = new List<CancellationTokenSource>();

    private bool isVehicle = false;
    private bool vehicleIsActive = false;
    private NewtonianSpaceShipInterface vehicleMono = null;

    private int counter;

    private void Start()
    {
        if(gameObject.TryGetComponent(out NewtonianSpaceShipInterface mono))
        {
            this.vehicleMono = mono;
            this.isVehicle = true;
            mono.handling.HasActivated += this.VehicleHasActivated;
            mono.handling.HasDeactivated += this.VehicleHasDeactivated;
        }  

        ms = GameObject.Find("Main").GetComponent<Main>();
        this.myRenderer = gameObject.GetComponent<Renderer>();
        this.originalColor = myRenderer.material.color;
    }

    public void SetUp(int id)
    {
        this.id = id;
    }
    #endregion

    #region MAUSE_INPUT
    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.selected == true)
        {
            this.ms.RegisterDeselection(this.id);
            this.selected = false;
            this.PaintUnselected();
        }
        else
        {
            ms.RegisterSelection(this.id);
        }
    }
    #endregion

    ///This method gets called from Main on the whole array of targets with the id of the selected target
    public void VisualiseSelection(int id)
    {
        ///If the id is on the current target it is painted green and marked selected
        if (this.id == id) /// selection 
        {
            this.selected = true;
            this.PaintSelected();
        }
        ///If it is another target and this target was the prevous selected one it is painted to 
        ///the original color and marked unselected
        else if (this.selected == true) /// deselection
        {
            this.selected = false;
            this.PaintUnselected();
        }
    }

    private void PaintSelected()
    {
        ///when selected the original color is saved and the selection color applyed
        this.originalColor = myRenderer.material.color;
        this.myRenderer.material.color = this.selectionColor;
    }

    private void PaintUnselected()
    {
        ///in case we are unselected mid flash; not sure if necessary
        if (this.myRenderer.material.color != this.selectionColor)
        {
            this.originalColor = this.myRenderer.material.color;
        }

        this.myRenderer.material.color = originalColor;
    }

    /// It gets called N+1 (the color change number) times because first it detects not selection color and then new color on the first two checks
    private void LateUpdate()
    {
        if(this.selected == false)
        {
            return;
        }

        if(this.isVehicle && this.vehicleIsActive)
        {
            return;
        }        

        if (this.ColorChangedCheck())
        {
            ///Flash is a preiod of time after color change for selected target that the color is showd
            ///after flash the selection color is applied again
            
            ///if flash AND Changed Color then we update the original color
            if (this.flash)
            {
                this.originalColor = myRenderer.material.color;
            }

            ///here we stargt a new flash and cancel previous flashes
            this.flash = true;
            var s = new CancellationTokenSource();
            var task = this.FlashNewColor(s.Token, this.counter++);
            this.CancelAsyncOperations();
            this.cancelationSources.Add(s);
        }
    }

    private bool ColorChangedCheck()
    {
        bool check = false;
        ///if flash we compare the previous Original Color with the currant color;
        ///if different there has been a color change
        if (this.flash)
        {
            check = this.originalColor != myRenderer.material.color;
        }
        ///else we check if the item is in selection color, if not there has been a change
        else
        {
            check = this.selectionColor != myRenderer.material.color;
        }

        return check;
    }

    ///cancels the all previous flashes, I do not thing I need an array for that, but good enough for now
    private void CancelAsyncOperations()
    {
        foreach (var s in this.cancelationSources)
        {
            if (s.IsCancellationRequested == false)
            {
                //Debug.Log("Canceled: " + s.Token.ToString());
                s.Cancel();
            }
        }

        this.cancelationSources.Clear();
    }

    ///Flashes the new color if the target gets its color changed while selected
    ///This is canceled if new color comes through so the new collor is flashed 
    ///for the same duration 600ms
    private async Task FlashNewColor(CancellationToken t, int counter)
    {
        await Task.Delay(600); /// flashing for 600 miliseconds

        ///if the task has been canceled do NOT revert the color to selection color
        if (t.IsCancellationRequested) 
        {
            return;
        }

        /// noting that a flash has ended
        flash = false; 
        /// the target has been deselected we do NOT paint it green at the end of the flash
        if (this.selected == false) 
        {
            return;
        }

        ///painting it back to selection color at the end of a successul flash
        this.myRenderer.material.color = this.selectionColor;
    }

    ///methods that subscribe to the Vehiche activated and deactivated events
    public void VehicleHasActivated()
    {
        if(this.selected == false)
        {
            return; 
        }

        this.vehicleIsActive = true;
        this.myRenderer.material.color = this.originalColor;
    }

    public void VehicleHasDeactivated()
    {
        if (this.selected == false)
        {
            return; 
        }

        this.vehicleIsActive = false;
        this.myRenderer.material.color = this.selectionColor; 
    }
    ///...
}
