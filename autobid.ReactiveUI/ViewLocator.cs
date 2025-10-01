using autobid.ReactiveUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace autobid.ReactiveUI
{
	public class ViewLocator : IDataTemplate
	{

		public Control? Build(object? param)
		{
			if (param is null)
				return null;

            var name = param.GetType().FullName!
				.Replace(".ViewModels.", ".Views.")
				.Replace("ViewModel", "View", StringComparison.Ordinal);



            var type = Type.GetType(name);

			if (type != null)
			{
				return (Control)Activator.CreateInstance(type)!;
			}

			return new TextBlock { Text = "Not Found: " + name };
		}

		public bool Match(object? data)
		{
			return data is ViewModelBase;
		}
	}
}
