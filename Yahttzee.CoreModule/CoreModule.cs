using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using NSubstitute;
using Yahtzee.Core;
using Ninject.MockingKernel;
using Yahtzee.Randomizer;

namespace Yahtzee.CoreModule
{
    public class CoreModule : NinjectModule
    {
        //private readonly MockingKernel _kernel;

        //public CoreModule()
        //{
        //    _kernel = new MockingKernel();
        //    _kernel.Bind<IRandomizer>().to<
        //}
        public override void Load()
        {
            //var kernel = new StandardKernel();
            //// Set up kernel

            //var substitute = Substitute.For<IRandomizer>();
            //kernel.Bind<IRandomizer>().ToContstant(substitute);
            //Bind<IRandomizer>().ToProvider<RandomizerProvider>();
            //Bind<IRandomizer>().To<Randomizer.Randomizer>();
        }
    }

    //public class RandomizerProvider : Provider<IRandomizer>
    //{
    //    protected override IRandomizer CreateInstance(IContext context)
    //    {
    //        IRandomizer randomizer = Substitute.For<IRandomizer>();
    //        randomizer.Roll(1, 6).Returns(1);
    //        return randomizer;
    //    }
    //}
}
