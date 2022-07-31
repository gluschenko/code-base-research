using Wishmaster.DataAccess.Models;

namespace Wishmaster.ViewModels
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
