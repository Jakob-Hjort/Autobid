using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.Controls
{
    public class DecimalTextBox : TextBox
    {
        protected override void OnTextInput(TextInputEventArgs e)
        {
            // Allow digits and one decimal point
            if (!decimal.TryParse(e.Text, out _) && e.Text != ".")
            {
                e.Handled = true;
            }
            base.OnTextInput(e);
        }
    }
}
