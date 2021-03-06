using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Xamarin.Forms;
using MugenMvvmToolkit.Xamarin.Forms.Infrastructure;

namespace Validation
{
    public class App : MvvmXamarinApplicationBase
    {
        #region Constructors

        public App(XamarinFormsBootstrapperBase.IPlatformService platformService)
            : base(platformService)
        {
        }

        #endregion

        #region Methods

        protected override XamarinFormsBootstrapperBase CreateBootstrapper(XamarinFormsBootstrapperBase.IPlatformService platformService, IDataContext context)
        {
            //note force load assembly
            var type = typeof(MugenMvvmToolkit.Xamarin.Forms.Binding.AttachedMembers);
            return new Bootstrapper<Portable.App>(platformService, new MugenContainer());
        }

        #endregion
    }
}