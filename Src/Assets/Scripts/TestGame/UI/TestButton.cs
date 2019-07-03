using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    private Main ms;
    private ReferenceBuffer rb;
    private Assembly assembly;

    private int counter = 0;

    private void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(this.OnClick);

        var main = GameObject.Find("Main");
        this.ms = main.GetComponent<Main>();
        this.rb = main.GetComponent<ReferenceBuffer>();
    }

    public void OnClick()
    {
        this.AssembliPipelineWorking();
    }

    public void AssembliPipelineWorking() ///true
    {
        if (counter == 0)
        {
            var text = this.rb.TextEditorInputField.text;
            Debug.Log("TEXT: " + text);
            this.assembly = Compilation.GenerateAssembly(text, false);
            counter++;
        }
        else if (counter == 1)
        {
            Compilation.WriteAssembly(this.assembly, Compilation.pleyerLevelAssembliesPath, "testyTest");
            counter++;
        }
        else if (counter == 2)
        {
            var loadedAssembly = Compilation.LoadAssembly(Compilation.pleyerLevelAssembliesPath, "testyTest");
            var funcs = Compilation.GenerateAllMethodsFromAssembly(loadedAssembly);
            var script = this.ms.AttachMono(funcs);
            counter++;
        }
        else if (counter == 3)
        {
            Compilation.DeleteAssembly(Compilation.pleyerLevelAssembliesPath, "testyTest");
        }
    }
}

