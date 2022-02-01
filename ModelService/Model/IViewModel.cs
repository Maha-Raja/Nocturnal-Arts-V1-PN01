using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model
{
    interface IViewModel<TEFModel, TViewModel>
    {
        void FromEFModel(TEFModel efModel, TViewModel viewModel);
        TEFModel ToEFModel(TViewModel viewModel);
    }
}
