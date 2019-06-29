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

    private int counter;

    private void Start()
    {
        ms = GameObject.Find("Main").GetComponent<Main>();
        this.myRenderer = gameObject.GetComponent<Renderer>();
        this.originalColor = myRenderer.material.color;
    }

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

    public void SetUp(int id)
    {
        this.id = id;
    }

    public void VisualiseSelection(int id)
    {
        if (this.id == id) // selection
        {
            this.selected = true;
            this.PaintSelected();
        }
        else if (this.selected == true) // deselection
        {
            this.selected = false;
            this.PaintUnselected();
        }
    }

    private void PaintSelected()
    {
        this.originalColor = myRenderer.material.color;
        this.myRenderer.material.color = this.selectionColor;
    }

    private void PaintUnselected()
    {
        if (this.myRenderer.material.color != this.selectionColor)
        {
            this.originalColor = this.myRenderer.material.color;
        }

        this.myRenderer.material.color = originalColor;
    }

    private void Update()
    {
        /// It gets called N+1 times because first it detects not selection color
        /// and then new color on the first two checks
        bool check = false;
        if (this.flash)
        {
            check = this.originalColor != myRenderer.material.color;
        }
        else
        {
            check = this.selectionColor != myRenderer.material.color;
        }

        if (this.selected && check)
        {
            if (this.flash)
            {
                this.originalColor = myRenderer.material.color;
            }

            this.flash = true;
            var s = new CancellationTokenSource();
            var task = this.FlashNewColor(s.Token, this.counter++);
            this.CancelAsyncOperations();
            this.cancelationSources.Add(s);

        }
    }

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

    private async Task FlashNewColor(CancellationToken t, int counter)
    {
        await Task.Delay(600);

        if (t.IsCancellationRequested)
        {
            //Debug.Log("Async Operation Canceled! " + counter);
            return;
        }

        flash = false;

        if (this.selected == false)
        {
            return;
        }

        //Debug.Log("Asunc Operation Made it! " + counter);

        this.myRenderer.material.color = this.selectionColor;
    }
}
