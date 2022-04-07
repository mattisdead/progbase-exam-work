using Terminal.Gui;

namespace ConsoleApp
{
    public class GraphDialog : Dialog
    {
        public static Label fileLabel;
        public bool _canceled;
        public GraphDialog()
        {
            this.Title = "Graph generator";

            Button okBtn = new Button("Create Graph");
            okBtn.Clicked += OnCreateGraphSubmitted;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateGraphCancelled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Button selectFileBtn = new Button(2, 2, "Open file");
            selectFileBtn.Clicked += SelectFile;

            fileLabel = new Label("default")
            {
                X = Pos.Right(selectFileBtn) + 2,
                Y = Pos.Top(selectFileBtn),
                Width = Dim.Fill() - 2,
            };

            this.Add(selectFileBtn, fileLabel);
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
        private void OnCreateGraphCancelled()
        {
            this._canceled = true;
            Application.RequestStop();
        }
        private void OnCreateGraphSubmitted()
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