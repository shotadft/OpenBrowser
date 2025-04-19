namespace OpenBrowser.Data
{
    public class PageHistory
    {
        public Dictionary<int, Uri?> CurrentUrl { get; set; } = new();
        public Dictionary<int, Stack<Uri>> UndoList { get; set; } = new();
        public Dictionary<int, Stack<Uri>> RedoList { get; set; } = new();

        public void Navigate(int pageId, Uri newUri)
        {
            if (!UndoList.ContainsKey(pageId))
            {
                UndoList[pageId] = new Stack<Uri>();
                RedoList[pageId] = new Stack<Uri>();
            }

            if (CurrentUrl.ContainsKey(pageId) && CurrentUrl[pageId] != null)
            {
                UndoList[pageId].Push(CurrentUrl[pageId]!);
            }

            CurrentUrl[pageId] = newUri;
            RedoList[pageId].Clear();
        }

        public Uri? Undo(int tabIndex)
        {
            if (UndoList.ContainsKey(tabIndex) && UndoList[tabIndex].Count > 0)
            {
                var current = CurrentUrl[tabIndex];
                var prev = UndoList[tabIndex].Pop();

                if (current != null)
                {
                    RedoList[tabIndex].Push(current);
                }

                CurrentUrl[tabIndex] = prev;
                return prev;
            }
            return null;
        }

        public Uri? Redo(int tabIndex)
        {
            if (RedoList.ContainsKey(tabIndex) && RedoList[tabIndex].Count > 0)
            {
                var current = CurrentUrl[tabIndex];
                var next = RedoList[tabIndex].Pop();

                if (current != null)
                {
                    UndoList[tabIndex].Push(current);
                }

                CurrentUrl[tabIndex] = next;
                return next;
            }
            return null;
        }

        public Uri? GetCurrent(int tabIndex)
        {
            return CurrentUrl.ContainsKey(tabIndex) ? CurrentUrl[tabIndex] : null;
        }
    }
}
