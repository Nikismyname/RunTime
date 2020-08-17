using UnityEngine;

public class ReferenceBuffer : MonoBehaviour
{
    public static ReferenceBuffer Instance { get; set; }
    public LevelBase Level { get; set; }
    public ShowCodeBehaviour ShowCode { get; set; }
    public ShowActionsBehaviour ShowActions { get; set; }
    public ShowAvailableCSFiles ShowAvailableCSFiles { get; set; }
    public ShowPresetsBehaviour ShowPresetBehaviour { get; set; }
    public GameObject ColorPicker { get; set; }
    public PlayerHandling2 PlayerHandling { get; set; }
    public GameObject PlayerObject { get; set; }
    public GameObject InfoTextObject { get; set; }
    public GameObject InfoTextCanvasGroup { get; set; }
    public InputFocusManager focusManager { get; set; }
    public TargetManagerBehaviour ms { get; set; }
    public GenerateLevel gl { get; set; }
    public GridManager gm { get; set; }
    public LevelManager LevelManager { get; set; }
    public CodeApplicator capp { get; set; }
    public MySceneManager MySceneManager { get; set; }
    public RBUiStateManager UIManager { get; set; }
    public UniUIManager UniUIManager { get; set; }
    public EditorInput EditorInput { get; set; }
    public GenerateUpdateAndDisplayTargetSpeceficUI ManageProcUI { get; set; }
    public GameObject UI { get; set; }
    public WorldSpaceUI worldSpaceUI { get; set; }
    public CamCenterIntersection PlayerIntersection { get; set; }
    public CamCenterIntersection DroneIntersection { get; set; }
    public Camera Camera { get; set; }

    private void Awake()
    {
        Instance = this;
        GameObject main = GameObject.Find("Main");
        this.ShowCode = GameObject.Find("ShowCodeButton").GetComponent<ShowCodeBehaviour>();
        this.ColorPicker = GameObject.Find("ColorPicker");
        this.ColorPicker.SetActive(false);
        this.ShowActions = GameObject.Find("ShowActionsButton").GetComponent<ShowActionsBehaviour>();
        this.ShowAvailableCSFiles = GameObject.Find("ShowAvailableFilesButton").GetComponent<ShowAvailableCSFiles>();
        this.ShowPresetBehaviour = GameObject.Find("ShwoPresetsButton").GetComponent<ShowPresetsBehaviour>();
        this.InfoTextObject = GameObject.Find("InfoText");
        this.InfoTextCanvasGroup = GameObject.Find("ScrollableInfoText");
        this.ms = main.GetComponent<TargetManagerBehaviour>();
        this.gm = main.GetComponent<GridManager>();
        this.gl = new GenerateLevel(this.ms, this, this.gm);
        this.LevelManager = main.GetComponent<LevelManager>();
        this.capp = new CodeApplicator();
        this.focusManager = main.GetComponent<InputFocusManager>();
        this.MySceneManager = GameObject.Find("SceneManager")?.GetComponent<MySceneManager>();
        this.UIManager = new RBUiStateManager(this);
        this.UniUIManager = main.GetComponent<UniUIManager>();
        this.EditorInput = main.GetComponent<EditorInput>();
        GameObject actionsContent = GameObject.Find("ActionsContent"); 
        this.ManageProcUI = actionsContent.GetComponent<GenerateUpdateAndDisplayTargetSpeceficUI>();
        this.UI = GameObject.Find("UI");
        this.Camera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    public void RegisterWorldSapceUI(WorldSpaceUI worldSpaceUI)
    {
        this.worldSpaceUI = worldSpaceUI;
    }

    public void RegisterPlayerHandling(PlayerHandling2 playerHandling)
    {
        this.PlayerHandling = playerHandling;
    }

    public void RegisterPlayerIntersection(CamCenterIntersection intersection)
    {
        this.PlayerIntersection = intersection;
    }
    
    public void RegisterDroneIntersection(CamCenterIntersection intersection)
    {
        this.DroneIntersection = intersection;
    }
}
