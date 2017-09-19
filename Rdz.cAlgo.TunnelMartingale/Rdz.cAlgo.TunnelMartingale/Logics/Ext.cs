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
		private List<Position> TMPositions
		{
			get
			{
				return (from iRow in Positions where iRow.Label.StartsWith(MainLabel) select iRow).ToList();
			}
		}
		private List<PendingOrder> TMPendingOrders
		{
			get
			{
				return (from iRow in PendingOrders where iRow.Label.StartsWith(MainLabel) select iRow).ToList();
			}
		}

		private bool IsFreshStart
		{
			get
			{
				return (TMPositions.Count == 0 && TMPendingOrders.Count == 0);
			}
		}
		private bool IsSecondPhase
		{
			get
			{
				return (TMPositions.Count == 1 && TMPendingOrders.Count == 1 && Properties.LastIndex == 0);
			}
		}
		private bool NoPendingOrders
		{
			get
			{
				return (TMPositions.Count > 0 && TMPendingOrders.Count == 0);
			}
		}
		private bool IdlePhase
		{
			get
			{
				return (TMPositions.Count > 0 && TMPendingOrders.Count == 1);
			}
		}

		private double Profit
		{
			get
			{
				return TMPositions.Sum(x => x.NetProfit);
			}
		}

		private void TakeCycleSnapshot()
		{
			Properties.Ask = this.Symbol.Ask;
			Properties.Bid = this.Symbol.Bid;
		}
		private void InitializeCycle()
		{
			if (StartOrder == enStartOrder.BuyFirst)
				Properties.LastExecutedTradeType = TradeType.Sell;
			else if (StartOrder == enStartOrder.SellFirst)
				Properties.LastExecutedTradeType = TradeType.Buy;
			Properties.Roof = Utility.ShiftPrice(Properties.Ask, (int)(TunnelSpanPoints / 2));
			Properties.Floor = Utility.ShiftPrice(Properties.Bid, -(int)(TunnelSpanPoints / 2));
			Properties.Stages = enStages.Idle;
			Properties.LastIndex = 0;
		}
		private void ResetCycle()
		{
			CancelAllPendingOrders();
			CloseAllPositions();
			Properties.Ask = Properties.Bid = Properties.Roof = Properties.Floor = Properties.LastIndex = 0;
		}
		private void PrintTradeResult(TradeResult res)
		{
			Print("Status: {0} {1} {2}{3}",
				(res.IsSuccessful ? "OK" : "FAILED"),
				(res.Error.HasValue ? "ERROR: " + res.Error.Value.ToString() : ""),
				(res.Position != null ? string.Format("(PosID: {0}; Label: {1}; Volume: {2})", res.Position.Id.ToString(), res.Position.Label, res.Position.Volume.ToString("#,##0")) : ""),
				(res.PendingOrder != null ? string.Format("(PendingOrderID: {0}; Label: {1}; Volume: {2})", res.PendingOrder.Id.ToString(), res.PendingOrder.Label, res.PendingOrder.Volume.ToString("#,##0")) : ""));
		}
		private void CancelAllPendingOrders()
		{
			Print("CancelAllPendingOrders");
			foreach (var TMPendingOrder in TMPendingOrders)
			{
				PrintTradeResult(CancelPendingOrder(TMPendingOrder));
			}
		}

		private void CloseAllPositions()
		{
			Print("CloseAllPositions");
			foreach (var TMPosition in TMPositions)
			{
				PrintTradeResult(ClosePosition(TMPosition));
			}
		}

		private void Controller()
		{
			TradeResult res;
			if (IsFreshStart && Properties.Stages == enStages.Idle)
			{
				Print("{0}: IsFreshStart", Properties.LastIndex.ToString());
				Properties.Stages = enStages.FreshStart;
				var tr1 = new TMTransaction(MainLabel, Properties.LastIndex);
				var tr2 = new TMTransaction(MainLabel, Properties.LastIndex);

				res = PlaceStopOrder(TradeType.Buy, Symbol, Utility.LotSizeToVolume(tr1.LotSize(LotSizeMultiplier, InitialLotSize)), Properties.Roof, tr1.Label);
				if (res.IsSuccessful) tr1.PendingOrderId = res.PendingOrder.Id;
				Properties.Transactions.Add(tr1);
				PrintTradeResult(res);

				res = PlaceStopOrder(TradeType.Sell, Symbol, Utility.LotSizeToVolume(tr2.LotSize(LotSizeMultiplier, InitialLotSize)), Properties.Floor, tr2.Label);
				if (res.IsSuccessful) tr2.PendingOrderId = res.PendingOrder.Id;
				Properties.Transactions.Add(tr2);
				PrintTradeResult(res);
				Properties.Stages = enStages.Idle;
			}
			else
			{
				if (IsSecondPhase && Properties.Stages == enStages.Idle)
				{
					Properties.LastIndex += 1;
					Print("{0}: IsSecondPhase", Properties.LastIndex.ToString());
					Properties.Stages = enStages.SecondPhase;
					CancelAllPendingOrders();

					Properties.LastExecutedTradeType = TMPositions.First().TradeType;
					Properties.LastExecutedTradeType = (Properties.LastExecutedTradeType == TradeType.Buy ? TradeType.Sell : TradeType.Buy);

					var tr = new TMTransaction(MainLabel, Properties.LastIndex);


					res = PlaceStopOrder(
						Properties.LastExecutedTradeType,
						Symbol,
						Utility.LotSizeToVolume(tr.LotSize(LotSizeMultiplier, InitialLotSize)),
						(Properties.LastExecutedTradeType == TradeType.Buy ? Properties.Roof : Properties.Floor),
						tr.Label);
					if (res.IsSuccessful) tr.PendingOrderId = res.PendingOrder.Id;
					Properties.Transactions.Add(tr);
					PrintTradeResult(res);
					Properties.Stages = enStages.Idle;
				}
				else if (NoPendingOrders && Properties.Stages == enStages.Idle)
				{
					Properties.LastIndex += 1;
					Print("{0}: NoPendingOrders", Properties.LastIndex.ToString());
					Properties.Stages = enStages.NoPendingOrders;
					Properties.LastExecutedTradeType = (Properties.LastExecutedTradeType == TradeType.Buy ? TradeType.Sell : TradeType.Buy);
					var tr = new TMTransaction(MainLabel, Properties.LastIndex);
					res = PlaceStopOrder(
						Properties.LastExecutedTradeType,
						Symbol,
						Utility.LotSizeToVolume(tr.LotSize(LotSizeMultiplier, InitialLotSize)),
						(Properties.LastExecutedTradeType == TradeType.Buy ? Properties.Roof : Properties.Floor),
						tr.Label);
					if (res.IsSuccessful) tr.PendingOrderId = res.PendingOrder.Id;
					Properties.Transactions.Add(tr);
					PrintTradeResult(res);
					Properties.Stages = enStages.Idle;
				}
				else if (IdlePhase)
				{
					if (Profit > TakeProfitCurrency)
					{
						Print("Profit: {0}", Profit.ToString("#.00"));
						CloseAllPositions();
						CancelAllPendingOrders();
						ResetCycle();
					}
				}
			}
		}
	}
}
