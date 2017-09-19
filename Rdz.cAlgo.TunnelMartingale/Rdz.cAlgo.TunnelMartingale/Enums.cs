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
		public enum enStartOrder
		{
			BuyFirst = 1,
			SellFirst = 2,
			Ranging = 3
		}

		public enum enBehavior
		{
			Normal = 1,
			Mode1 = 2
		}

		public enum enStages
		{
			FreshStart = 0,
			Idle = 1,
			SecondPhase = 2,
			NoPendingOrders = 3
		}
	}
}
