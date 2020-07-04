public class RBUiStateManager
{
    private ReferenceBuffer rb;

    public RBUiStateManager(ReferenceBuffer rb )
    {
        this.rb = rb;
    }

    public void CloseActionsMenu()
    {
        this.rb.ShowActions.Close();
    }

    public void OpenActionsMenu()
    {
        this.rb.ShowActions.Open();
    }

    public void CloseFileSelection()
    {
        this.rb.ShowAvailableCSFiles.Close();
    }

    public void OpenFilesSelection()
    {
        this.rb.ShowAvailableCSFiles.Open();
    }

    public void OpenCodeEditor()
    {
        this.rb.ShowCode.Open();
    }

    public void CloseCodeEditor()
    {
        this.rb.ShowCode.Close();
    }
}

