using System;
using System.Net;

class TimeoutWebClient : WebClient
{
	protected override WebRequest GetWebRequest(Uri uri)
	{
		WebRequest w = base.GetWebRequest(uri);
		w.Timeout = 5000;
		return w;
	}
}
