using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Core;

namespace Yahtzee.ViewModels
{
    public class BindingModule
    {
        private IRandomizer _randomizer;
        public BindingModule(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public IRandomizer GetRandomizer { get { return _randomizer; } }
    }
}
