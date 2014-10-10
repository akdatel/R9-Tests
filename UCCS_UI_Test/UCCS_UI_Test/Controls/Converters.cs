using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using UCCS_UI_Test.Models;

namespace UCCS_UI_Test.Controls
{
	[ValueConversion(typeof(AgentStatus), typeof(Brush))]
	public class AgentStateToColorConverter : IValueConverter
	{
		// http://www.codeproject.com/Articles/15061/Piping-Value-Converters-in-WPF

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is AgentStatus, "value should be an AgentStatus");
			Debug.Assert(targetType == typeof(Brush), "targetType should be Brush");

			switch ((AgentStatus)value)
			{
				case AgentStatus.Busy:
					return Brushes.LightGray;

				case AgentStatus.Handle:
					return Brushes.Lavender;

				case AgentStatus.Idle:
					return Brushes.White;

				case AgentStatus.LoggedOut:
					return Brushes.DarkSalmon;

				case AgentStatus.UnavailableIn:
				case AgentStatus.UnavailableOut:
					return Brushes.DarkSeaGreen;
			}
			return Colors.Transparent;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("ConvertBack not supported.");
		}
	} 
}
