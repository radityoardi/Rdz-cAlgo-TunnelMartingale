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


namespace Rdz.cAlgo.TunnelMartingale.Logics
{
	public partial class TunnelMartingalecBot : RdzRobot
	{
		private void Initialize()
		{
			Print("..:: Starting GREEDYGRID © 2017 by Rdz ::..");
		}
		private void Deinitialize()
		{
			Print("..:: Deinitialize GREEDYGRID © 2017 by Rdz ::..");
		}
	}
}
