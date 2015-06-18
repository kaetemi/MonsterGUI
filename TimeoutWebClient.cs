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
		HttpWebRequest w = base.GetWebRequest(uri) as HttpWebRequest;
		w.Timeout = 5000;
		w.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
		w.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
		return w;
	}
}

class TimeoutWebClientFast : TimeoutWebClient
{
	protected override WebRequest GetWebRequest(Uri uri)
	{
		HttpWebRequest w = base.GetWebRequest(uri) as HttpWebRequest;
		w.Timeout = 100;
		return w;
	}
}
