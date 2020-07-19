#region INIT

using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectionsRegisterer
{
    private ConnectionsTracker connTracker;
    private InputCanvas inputCanvas;
    private LineDrawer drawer;
    private ResultNode resultNode;

    private MethodNode lastClickedMethod = null;
    private PropertyNode lastClickedProperty = null;
    private ParameterNode lastClickedParameter = null;


    public ConnectionsRegisterer(ConnectionsTracker connTracker, InputCanvas inputCanvas, LineDrawer drawer, ResultNode resultNode)
    {
        this.connTracker = connTracker;
        this.inputCanvas = inputCanvas;
        this.drawer = drawer;
        this.resultNode = resultNode;
    }

    #endregion

    public void RegisterConstantClick(DirectInputNode node, ParameterNode paraNodeIn = null)
    {
        var paraNode = this.lastClickedParameter;
        if (paraNodeIn != null)
        {
            paraNode = paraNodeIn;
        }

        if (paraNode != null)
        {
            if (node.elements.Used)
            {
                Debug.Log("constant already used");
                return;
            }

            this.connTracker.TrackParameterAssignConstant(paraNode, node);

            this.inputCanvas.InputsHide();

            paraNode.RegisterAssignment();

            InputCanvas.InputElements n = this.inputCanvas.GetInputs().Single(x => x.Node == node);

            n.Used = true;
            n.Node.SetUsed(true, paraNode);

            return;
        }

        if (this.lastClickedProperty != null)
        {
            ///TODO:
        }
    }

    public void RegisterResultClick(ResultNode resultNode, MethodNode methodNodeIn = null)
    {
        var methodNode = this.lastClickedMethod;
        if (methodNodeIn != null)
        {
            methodNode = methodNodeIn;
        }

        if (methodNode == null)
        {
            return;
        }

        this.connTracker.TrackResultAssignMethodCall(methodNode);

        this.DrawConnection(this.resultNode.gameObject, methodNode.gameObject);

        this.lastClickedMethod = null;
    }


    public void RegisterPropertyClick(PropertyNode node)
    {
        this.lastClickedProperty = node;
        /// DO IT!
    }


    public async Task RegisterParameterClick(ParameterNode node, MethodNode methodNodeIn = null)
    {
        /// Cashing the last clicked parameter
        this.lastClickedParameter = node;

        /// Makes it gray!
        this.lastClickedParameter.RegisterSelection();

        /// This allows us to pass external methodNode!
        var methodNode = this.lastClickedMethod;
        if (methodNodeIn != null)
        {
            methodNode = methodNodeIn;
        }
        ///...

        /// if not method to connect the property with - end the method and only then show the inputCanvas
        if (methodNode == null)
        {
            Debug.Log($"RegisterParameterClick methodNode NULL");
            await this.inputCanvas.InputsDisplay(node.transform.position, node);
            return;
        }

        /// Trying to short circuit pram to its method return
        if (node.Object == methodNode.Object && node.myMethod == methodNode.MyMethodInfo.Info)
        {
            Debug.Log($"RegisterParameterClick SHORT CIRCUIT");
            return;
        }

        Debug.Log($"RegisterParameterClick CONNECTION MADE");
        this.connTracker.TrackParameterAssignMethod(node, methodNode);
        this.DrawConnection(node.gameObject, methodNode.gameObject);
        ///Reset the srelevent selection after connection
        this.lastClickedParameter = null;
        this.lastClickedMethod = null;
    }


    public void RegisterMethodClick(MethodNode node, ParameterNode paramNodeIn = null)
    {
        this.lastClickedMethod = node;

        var paramNode = this.lastClickedParameter;
        if (paramNodeIn != null)
        {
            paramNode = paramNodeIn;
        }

        if (paramNode == null)
        {
            Debug.Log($"RegisterMethodClick paramNode NULL");
            return;
        }

        /// Trying to short circuit pram to its method return
        if (paramNode.Object == node.Object && paramNode.myMethod == node.MyMethodInfo.Info)
        {
            Debug.Log($"RegisterMethodClick SHORT CIRCUIT");
            return;
        }

        Debug.Log($"RegisterMethodClick CONNECTION MADE");
        this.DrawConnection(node.gameObject, paramNode.gameObject);
        this.connTracker.TrackParameterAssignMethod(paramNode, node);
        ///Reset the srelevent selection after 
        this.lastClickedParameter = null;
        this.lastClickedMethod = null;
    }

    private void DrawConnection(GameObject one, GameObject two)
    {
        if (one.transform.parent == two.transform.parent)
        {
            this.drawer.DrawDynamicCurve(one.transform, two.transform, two.transform.parent.transform.position, 0.1f, Color.cyan, 1);
        }
        else
        {
            this.drawer.DrawDynamicLine(one.transform, two.transform, 0.1f, Color.green);
        }
    }

    public void ResetToNull()
    {
        this.lastClickedMethod = null;
        this.lastClickedParameter = null;
        this.lastClickedProperty = null;
    }
}
