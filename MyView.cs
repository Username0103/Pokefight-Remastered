namespace Pokefight_Remastered
{
    using Terminal.Gui;

    public partial class MyView
    {
        public MyView()
        {
            InitializeComponent();
            button1.MouseClick += (obj, args) => MessageBox.Query("Hello", "Hello There!", "Ok");
        }
    }
}
