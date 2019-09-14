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
    private GridManager gm;
    private LevelManager lm;
    private bool selected = false;
    private bool flash = false;
    private Renderer myRenderer;
    private Color originalColor;
    private Color selectionColor = Color.green;
    private List<CancellationTokenSource> cancelationSources = new List<CancellationTokenSource>();

    private bool isVehicle = false;
    private bool vehicleIsActive = false;
    private bool isGrid = false;
    private NewtonianSpaceShipInterface vehicleMono = null;

    public TargetType type;
    string testName;
    private bool solved = false;

    private int counter;

    private void Start()
    {
        /// If the target has ship behaviour get the mono and set up target as ship.
        if (gameObject.TryGetComponent(out NewtonianSpaceShipInterface mono))
        {
            this.vehicleMono = mono;
            this.isVehicle = true;
            mono.handling.HasActivated += this.VehicleHasActivated;
            mono.handling.HasDeactivated += this.VehicleHasDeactivated;
        }

        var main = GameObject.Find("Main"); 
        this.ms = main.GetComponent<Main>();
        this.gm = main.GetComponent<GridManager>();
        this.lm = main.GetComponent<LevelManager>(); 
        /// Saving the original color of the object.
        this.myRenderer = gameObject.GetComponent<Renderer>();
        this.originalColor = myRenderer.material.color;
    }

    /// <summary>
    /// Called From Main and GenerateLevel.Player()
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="testName"></param>
    /// <param name="isGrid"></param>
    public void SetUp(
        int id,
        TargetType type = TargetType.Standard,
        string testName = "",
        bool isGrid = false
    )
    {
        this.id = id;
        this.type = type;
        this.testName = testName;
        this.isGrid = isGrid;
    }
    #endregion

    #region TEST
    /// <summary>
    /// Called from ApplyBehaviour
    /// 
    /// Recives the assembly as bytes and sends it to the test app domain.
    /// This prevents the assembly to be loaded in the main app domain,
    /// making it posible to release the assembly memory.
    /// If the test passes, it returns true else false 
    /// </summary>
    public async Task<bool> Test(byte[] assembly)
    { 
        if(this.type != TargetType.Test)
        {
            Debug.Log("This is not a test target!");
            return false; 
        } 

        Debug.Log("name: " + this.testName);
        var result = false;
        if (isGrid)
        {
            result = Compilation.Loader.RTGridTest(assembly, this.testName, out int[][] selections);
            if (result == true)
            {
                await this.gm.LightTiles(selections);
                this.lm.Success();
            }
            else
            {
                await this.gm.LightTiles(selections);
            }
        }
        else
        {
            result = Compilation.Loader.RTTest(assembly, this.testName); 
        }

        if (result == true)
        {
            this.originalColor = Color.blue;
            this.solved = true; 
        }
        return result;

        //void PrintSelection(int[][] selection)
        //{
        //    string text = "";
        //    for (int i = 0; i < selection.Length; i++)
        //    {
        //        for (int j = 0; j < selection[i].Length; j+= 2)
        //        {
        //            text += $"{{{selection[i][j]}, {selection[i][j + 1]}}} "; 
        //        }
        //        text += '\n'; 
        //    }

        //    Debug.Log(text);
        //}
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

    #region VISUALISE_SELECTION
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
    #endregion

    #region MONITOR_FOR_COLOR_CHANGES
    /// It gets called N+1 (the color change number) times because first it detects not selection color and then new color on the first two checks
    private void LateUpdate()
    {
        if (this.selected == false)
        {
            return;
        }

        if (this.isVehicle && this.vehicleIsActive)
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
    #endregion

    #region CANCEL_FLASH_IF_ANOTHER_FLASH_INITIATED_LATER
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
    #endregion

    #region REVERT_NEW_COLOR_AFTER_FLASH 
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
    #endregion

    #region VEHICLE_ACTIVATION
    ///methods that subscribe to the Vehiche activated and deactivated events
    public void VehicleHasActivated()
    {
        if (this.selected == false)
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
    #endregion

    #region END_BRACKET
}
#endregion
