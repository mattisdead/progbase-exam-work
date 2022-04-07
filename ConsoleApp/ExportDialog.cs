using Terminal.Gui;
using Microsoft.Data.Sqlite;
using Entities;

namespace ConsoleApp
{
    public class ExportDialog : Dialog
    {
        public TextField searchInput;
        public static Label fileLabel;
        public bool _canceled;
        public ExportDialog()
        {
            this.Title = "Export";
            Button okBtn = new Button("Export");
            okBtn.Clicked += OnExportSubmitted;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnExportCancelled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label searchLbl = new Label(2, 4, "Search:");
            searchInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(searchLbl),
                Width = Dim.Fill() - 2,
            };

            Button selectFileBtn = new Button(2, 2, "Open file");
            selectFileBtn.Clicked += SelectFile;

            fileLabel = new Label("default")
            {
                X = Pos.Right(selectFileBtn) + 2,
                Y = Pos.Top(selectFileBtn),
                Width = Dim.Fill() - 2,
            };

            this.Add(searchLbl, searchInput, selectFileBtn, fileLabel);
        }
        static void SelectFile()
        {
            OpenDialog dialog = new OpenDialog("Open XML file", "Open?");

            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                NStack.ustring filePath = dialog.FilePath;
                fileLabel.Text = filePath;
            }
            else
            {
                fileLabel.Text = "not selected.";
            }
        }
        private void OnExportCancelled()
        {
            this._canceled = true;
            Application.RequestStop();
        }
        private void OnExportSubmitted()
        {
            this._canceled = false;
            Application.RequestStop();
        }
        public string GetFilePath()
        {
            string filePath;
            filePath = fileLabel.Text.ToString();
            return filePath;
        }
    }
}