using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeSplitManage
{
	// Delegate for passing received message back to caller
	public delegate void IntercommsMessageDelegate(string Message);

	public class IntercommsServer
	{
		public event IntercommsMessageDelegate IntercommsMessage;
		string _pipeName;

		public void Listen(string PipeName)
		{
			try
			{
				_pipeName = PipeName; // Set to class level var so we can re-use in the async callback method                 
				NamedPipeServerStream pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous); // Create the new async pipe                
				pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer); // Wait for a connection
			}
			catch (Exception oEX)
			{
				Debug.WriteLine("[Manage] Intercomms Server Listen Error, " + oEX.Message);
			}
		}

		private void WaitForConnectionCallBack(IAsyncResult iar)
		{
			try
			{
				NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState; // Get the pipe
				pipeServer.EndWaitForConnection(iar); // End waiting for the connection
				byte[] buffer = new byte[255];

				string stringData = "";
				while (pipeServer.Read(buffer, 0, 255) > 0) // Read the incoming message
				{
					int len = 0;
					while (len < 255 && buffer[len] != '\0')
						len++;
					stringData += Encoding.UTF8.GetString(buffer, 0, len/*buffer.Length*/); // Convert byte buffer to string
					buffer = new byte[255];
				}
				IntercommsMessage.Invoke(stringData); // Pass message back to calling form
				pipeServer.Close(); // Kill original sever and create new wait server
				pipeServer = null;
				pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
				pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer); // Recursively wait for the connection again and again....
			}
			catch (Exception oEX)
			{
				Debug.WriteLine("[Manage] Intercomms Server WaitForConnectionCallBack Error, " + oEX.Message);
				return;
			}
		}
	}
}
