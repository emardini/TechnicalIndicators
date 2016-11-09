namespace OANDARestLibrary.Framework
{
    public class CustomEventArgs<T>
    {
        #region Constructors and Destructors

        public CustomEventArgs(T content)
        {
            this.Item = content;
        }

        #endregion

        #region Public Properties

        public T Item { get; private set; }

        #endregion
    }
}