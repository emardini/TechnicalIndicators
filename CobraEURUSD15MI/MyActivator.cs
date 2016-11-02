namespace CobraEURUSD15MI
{
    using Microsoft.Azure.WebJobs.Host;

    using Ninject;

    public class MyActivator : IJobActivator
    {
        #region Fields

        private readonly IKernel container;

        #endregion

        #region Constructors and Destructors

        public MyActivator(IKernel container)
        {
            this.container = container;
        }

        #endregion

        #region Public Methods and Operators

        public T CreateInstance<T>()
        {
            return this.container.TryGet<T>();
        }

        #endregion
    }
}