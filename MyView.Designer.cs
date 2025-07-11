using System;
using Terminal.Gui;

namespace Pokefight_Remastered {
    
    public partial class MyView : Terminal.Gui.Window
    {
        private Terminal.Gui.Label label1;
        private Terminal.Gui.Button button1;

        private void InitializeComponent()
        {
            button1 = new Terminal.Gui.Button();
            label1 = new Terminal.Gui.Label();
            Width = Dim.Fill(0);
            Height = Dim.Fill(0);
            X = 0;
            Y = 0;
            Modal = false;
            Text = "";
            Title = "Press Ctrl+Q to quit";
            label1.Width = 4;
            label1.Height = 1;
            label1.X = Pos.Center();
            label1.Y = Pos.Center();
            label1.Data = "label1";
            label1.Text = "Hello World";
            Add(label1);
            button1.Width = 12;
            button1.X = Pos.Center();
            button1.Y = Pos.Center() + 1;
            button1.Data = "button1";
            button1.Text = "Click Me";
            button1.IsDefault = false;
            Add(button1);
        }
    }
}
