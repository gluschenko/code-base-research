using Wishmaster.DataAccess.Models;
using Wishmaster.ViewModels;

namespace Wishmaster.Backend.ViewModels
{
    public static class ViewModelExtensions
    {
        public static SpaceItemViewModel ToViewModel(this Space space)
        {
            return new SpaceItemViewModel
            {
                Uid = space.Uid,
                Name = space.Name,
            };
        }
    }
}
