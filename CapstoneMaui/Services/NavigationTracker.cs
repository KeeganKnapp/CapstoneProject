namespace CapstoneMaui.Services
{
    public class NavigationTracker
    {
        private Stack<string> uriStack = new Stack<string>();

        private string buttonLabel = "Back";

        public string ButtonLabel
        {
            get => buttonLabel;
            set => buttonLabel = value;
        }
        public int Count => uriStack.Count;
        public void Push(string uri)
        {
            uriStack.Push(uri);
        }

        public string Pop()
        {
            return uriStack.Pop();
        }

        public string Peek()
        {
            return uriStack.Peek();
        }

        public bool IsEmpty()
        {
            return !uriStack.Any();
        }
    }
}