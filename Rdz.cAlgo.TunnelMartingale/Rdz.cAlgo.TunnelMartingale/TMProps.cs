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
	internal class TMProps
	{
		internal double Ask { get; set; }
		internal double Bid { get; set; }
		internal double Roof { get; set; }
		internal double Floor { get; set; }
		internal TradeType LastExecutedTradeType { get; set; }
		internal int LastIndex { get; set; }
		internal TunnelMartingalecBot.enStages Stages { get; set; }

		private List<TMTransaction> _trans;
		internal List<TMTransaction> Transactions
		{
			get
			{
				if (_trans == null) _trans = new List<TMTransaction>();
				return _trans;
			}
		}
	}

	internal class TMTransaction
	{
		public TMTransaction(string MainLabel, int OrderIndex)
		{
			ID = Guid.NewGuid();
			this.OrderIndex = OrderIndex;
			Label = string.Format("{0}-{1}-{2}", MainLabel, OrderIndex.ToString(), ID.ToString("N"));
		}
		internal Guid ID { get; set; }
		internal int PendingOrderId { get; set; }
		internal int PositionId { get; set; }
		internal int OrderIndex { get; set; }
		internal string Label { get; set; }
		internal double LotSize(double Multiplier, double InitialLotSize)
		{
			if (OrderIndex == 0)
			{
				return InitialLotSize;
			}
			else
			{
				return (Math.Pow(Multiplier, OrderIndex) * InitialLotSize);
			}
		}
	}
}
