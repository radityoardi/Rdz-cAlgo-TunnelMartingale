using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using Rdz.cAlgo.Library;


namespace Rdz.cAlgo.TunnelMartingale
{
	public partial class TunnelMartingalecBot : RdzRobot
	{
		[Parameter("Initial Lot Size", DefaultValue = 0.01, MinValue = 0.01)]
		public double InitialLotSize { get; set; }
		[Parameter("Take Profit (in currency)", DefaultValue = 1, MinValue = 0)]
		public double TakeProfitCurrency { get; set; }
		[Parameter("Tunnel Span (points)", DefaultValue = 300, MinValue = 1)]
		public int TunnelSpanPoints { get; set; }
		[Parameter("Lot Size Multiplier", DefaultValue = 2, MinValue = 1)]
		public double LotSizeMultiplier { get; set; }

		[Parameter("Start Order", DefaultValue = 3, MinValue = 1, MaxValue = 3)]
		public int INTStartOrder { get; set; }
		public enStartOrder StartOrder
		{
			get
			{
				return (enStartOrder)INTStartOrder;
			}
		}

		[Parameter("Use Time Constraints", DefaultValue = false)]
		public bool UseTimeConstraint { get; set; }
		[Parameter("Main Label", DefaultValue = "TM")]
		public string MainLabel { get; set; }
	}
}
