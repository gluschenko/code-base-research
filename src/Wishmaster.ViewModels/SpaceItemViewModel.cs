using System;

namespace Wishmaster.ViewModels
{
    public class SpaceItemViewModel
    {
        public Guid Uid { get; set; }
        public string? Name { get; set; }
        public bool IsChosen { get; set; }
        public string? Description => IsChosen ? "Chosen" : "";
    }

    public enum TestEnum
    {
        A = 1,
        B = 2
    }
}
