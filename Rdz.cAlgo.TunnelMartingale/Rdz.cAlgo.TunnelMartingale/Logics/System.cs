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
		#region Properties
		#region Hidden Properties
		TMProps _p;
		#endregion

		TMProps Properties {
			get
			{
				if (_p == null) _p = new TMProps();
				return _p;
			}
		}
		#endregion

		private void Initialize()
		{
			Print("..:: Rdz.cAlgo.TunnelMartingale © 2017 by Rdz (STARTED) ::..");
		}
		private void Deinitialize()
		{
			Print("..:: Rdz.cAlgo.TunnelMartingale © 2017 by Rdz (STOPPED) ::..");
		}
		private void Tick()
		{
			if (IsFreshStart)
			{
				TakeCycleSnapshot();
				InitializeCycle();
			}
			Controller();
		}
	}
}
