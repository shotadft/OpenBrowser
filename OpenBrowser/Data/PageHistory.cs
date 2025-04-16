namespace OpenBrowser.Data
{
    public class PageHistory
    {
        public Uri? CurrentUrl { get; set; } = null;
        public Stack<Uri> UndoList { get; set; } = new();
        public Stack<Uri> ForwardList { get; set; } = new();
    }
}
