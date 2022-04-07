using Terminal.Gui;

namespace ConsoleApp
{
    public class RandomDialog : Dialog
    {
        public TextField entityInput;
        public TextField numberInput;
        public bool _canceled;
        public RandomDialog()
        {
            this.Title = "Random";
            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmitted;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label entityLbl = new Label(2, 4, "Entity:");
            entityInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(entityLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(entityLbl, entityInput);

            Label numberLbl = new Label(2, 6, "Amount:");
            numberInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(numberLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(numberLbl, numberInput);
        }
        private void OnCreateDialogCancelled()
        {
            this._canceled = true;
            Application.RequestStop();
        }
        private void OnCreateDialogSubmitted()
        {
            this._canceled = false;
            Application.RequestStop();
        }
    }
}