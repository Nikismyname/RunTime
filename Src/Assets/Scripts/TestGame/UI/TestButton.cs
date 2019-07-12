using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    private Main ms;
    private ReferenceBuffer rb;
    private Assembly assembly;
    private string path;
    private bool shouldUpdate = false;

    public const string assetFolderPath = "C:/Users/ASUS G751JY/source/repos/11111 Unity/RunTime/Src/Assets";

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
        this.SwitchUpdate();
        //Compilation.FreeLibraryTest();
        //Compilation.UnloadCurrentAppDomain();
        //Compilation.AttachMonoFromSeparateDomain();
        //this.TestSeparateDomainLoading();
    }

    private void TestSeparateDomainLoading()
    {
        if (counter == 0)
        {
            var text = this.rb.TextEditorInputField.text;
            Debug.Log("TEXT: " + text);
            this.path = Compilation.GenerateAssemblyNewDomain(text);
            counter++;
        }
        else if (counter == 1)
        {
            Compilation.GetTypesFromAssemblySeparateDomain(this.path);
        }
    }

    private void SwitchUpdate()
    {
        if (this.shouldUpdate == false)
        {
            this.shouldUpdate = true;
            Debug.Log("Creating Garbage");
        }
        else
        {
            this.shouldUpdate = false;
        }
    }

    private void Update()
    {
        if (this.shouldUpdate)
        {
            Compilation.MemoryTest();
        }
    }

    private void AssembliPipelineWorking() ///true
    {
        if (counter == 0)
        {
            //var text = this.rb.TextEditorInputField.text;
            //Debug.Log("TEXT: " + text);
            //this.assembly = Compilation.GenerateAssembly(text, false);
            //counter++;
        }
        else if (counter == 1)
        {
            //Compilation.WriteAssemblyToFile(this.assembly, Compilation.PleyerLevelAssembliesPath, "testyTest");
            //counter++;
        }
        else if (counter == 2)
        {
            //var loadedAssembly = Compilation.LoadAssembly(Compilation.PleyerLevelAssembliesPath, "testyTest");
            //var funcs = Compilation.GenerateAllMethodsFromAssembly(loadedAssembly);
            //var script = this.ms.AttachMono(funcs);
            //counter++;
        }
        else if (counter == 3)
        {
            //Compilation.DeleteAssembly(Compilation.PleyerLevelAssembliesPath, "testyTest");
        }
    }
}

