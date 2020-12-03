using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarcodeSplitManage
{
	//
	// Summary:
	//     Identifies the type of event that has caused the trace.
	public enum TraceEventType
	{
		//
		// Summary:
		//     No log entries.
		None = 0,
		//
		// Summary:
		//     Fatal error or application crash.
		Critical = 1,
		//
		// Summary:
		//     Recoverable error.
		Error = 2,
		//
		// Summary:
		//     Noncritical problem.
		Warning = 4,
		//
		// Summary:
		//     Informational message.
		Information = 8,
		//
		// Summary:
		//     Debugging trace - all messages useful for debugging purposes.
		Debug = 16,
		//
		// Summary:
		//     Debugging trace and data transfer - log everything.
		Verbose = 4096
	}
}
