using System;
using System.Net;

class TimeoutWebClient : WebClient
{
	public TimeoutWebClient()
	{
		Encoding = System.Text.Encoding.UTF8;
	}

	protected override WebRequest GetWebRequest(Uri uri)
	{
		WebRequest w = base.GetWebRequest(uri);
		w.Timeout = 5000;
		return w;
	}
}
