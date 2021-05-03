using System;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeBase.Domain.Services
{
    public static class DialogHelper
    {
        public static void OpenFolderDialog(string path, Action<string> onChange)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = path,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dialog.FileName;
                onChange(folder);
            }
        }
    }
}
