using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeSplitManage
{
	public class IntercommsClient
	{
		public void Send(string SendStr, string PipeName, int TimeOut = 1000)
		{
			try
			{
				NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", PipeName, PipeDirection.Out, PipeOptions.Asynchronous);
				pipeStream.Connect(TimeOut);
				byte[] _buffer = Encoding.UTF8.GetBytes(SendStr);
				pipeStream.BeginWrite(_buffer, 0, _buffer.Length, AsyncSend, pipeStream);
			}
			catch (TimeoutException oEX)
			{
				Debug.WriteLine("[Manage] Intercomms Client Send Error, " + oEX.Message + ", " + SendStr + ", " + PipeName);
			}
		}

		private void AsyncSend(IAsyncResult iar)
		{
			try
			{
				NamedPipeClientStream pipeStream = (NamedPipeClientStream)iar.AsyncState; // Get the pipe
				pipeStream.EndWrite(iar); // End the write
				pipeStream.Flush();
				pipeStream.Close();
				pipeStream.Dispose();
			}
			catch (Exception oEX)
			{
				Debug.WriteLine("[Manage] Intercomms Client AsyncSend Error," + oEX.Message);
			}
		}
	}
}
