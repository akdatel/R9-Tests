using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSynchroTests
{
	class Program
	{
		private static object locker = new object();
	//	private static int complete = 0;				// #3 FIX OPTION (hmmm>???)

		static void Main(string[] args)
		{
			ThreadNeverFinishes_DueToCompilerOptimizationsInRelease();
		}

		/// <summary>
		/// FROM http://www.albahari.com/threading/part4.aspx
		/// This will BLOCK if run in release mode, from the EXE file (NOT from VS!)
		/// </summary>
		private static void ThreadNeverFinishes_DueToCompilerOptimizationsInRelease()
		{
			int complete = 0;
			Console.WriteLine(" int complete = " + complete);

			var t = new Thread (() =>
			{
				bool toggle = false;
				while (true)
				{
					//lock (locker) {				// #2.1 FIX OPTION
					if (complete == 1)	
				//	if (Interlocked.CompareExchange(ref complete, 0, 0) == 1)		// #3. FIX OPTION
							return;
					//	}							// #2.1 FIX OPTION

					toggle = !toggle;
				//  Thread.MemoryBarrier();			// #1 FIX option
				}
			});

			t.Start();

			Console.WriteLine("Thread.Sleep(5000)!");
			Thread.Sleep(5000);
			
			complete = 1;
			//  Interlocked.Exchange(ref complete, 1); //// THIS DOES NOT HELP!!!
			Console.WriteLine("complete = 1!");

			t.Join();        // Blocks indefinitely
			
			Console.WriteLine("DONE!!!");
			Console.ReadKey();

		}
	}
}
