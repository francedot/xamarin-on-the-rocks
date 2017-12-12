using FormsEmbedding.Helpers;
using FormsEmbedding.Interfaces;
using FormsEmbedding.Services;
using FormsEmbedding.Model;

namespace FormsEmbedding
{
    public partial class Bootstrapper
    {
        public Bootstrapper()
        {
        }

        /// <summary>
        /// DI Registration
        /// </summary>
        public static void Initialize()
        {
            ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
        }
    }
}