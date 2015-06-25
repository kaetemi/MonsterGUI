using System;
using System.Net;

class TimeoutWebClient : WebClient
{
	/*CookieContainer cc = new CookieContainer();
	public CookieContainer CookieContainer { get { return cc; } }
	*/

	public string Cookie = "";

	public TimeoutWebClient()
	{
		Encoding = System.Text.Encoding.UTF8;
	}

	protected override WebRequest GetWebRequest(Uri uri)
	{
		HttpWebRequest w = base.GetWebRequest(uri) as HttpWebRequest;
		w.Timeout = 5000;
		w.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
		w.Headers.Add(HttpRequestHeader.Cookie, Cookie);
		w.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
		return w;
	}
}
