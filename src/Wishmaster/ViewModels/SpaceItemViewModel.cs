using System;
using Wishmaster.DataAccess.Models;

namespace Wishmaster.ViewModels
{
    public class SpaceItemViewModel
    {
        public Guid Uid { get; set; }
        public string? Name { get; set; }
        public bool IsChosen { get; set; }
        public string? Description => IsChosen ? "Chosen" : "";

        public SpaceItemViewModel()
        {
        }

        public SpaceItemViewModel(Space space)
        {
            Uid = space.Uid;
            Name = space.Name;
        }
    }
}
