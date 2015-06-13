/*
Url Encoder
Copyright (C) 2006  Jan Boon
Author: Jan Boon <jan.boon@kaetemi.be>
Public domain
*/

using System;
using System.Text;

public static partial class WebUtilities
{
	public static ArraySegment<byte> UrlEncode(byte[] utf8)
	{
		byte[] result = new byte[utf8.Length * 3];
		int length = 0;
		foreach (byte b in utf8) switch (b)
		{
			case 0x21: // !
			case 0x27: // ' 
			case 0x28: // (
			case 0x29: // )
			case 0x2a: // *
			case 0x2d: // -
			case 0x2e: // .
			case 0x30: // 0-9 to 39
			case 0x31:
			case 0x32:
			case 0x33:
			case 0x34:
			case 0x35:
			case 0x36:
			case 0x37:
			case 0x38:
			case 0x39:
			case 0x41: // a-z to 5a
			case 0x42:
			case 0x43:
			case 0x44:
			case 0x45:
			case 0x46:
			case 0x47:
			case 0x48:
			case 0x49:
			case 0x4a:
			case 0x4b:
			case 0x4c:
			case 0x4d:
			case 0x4e:
			case 0x4f:
			case 0x50:
			case 0x51:
			case 0x52:
			case 0x53:
			case 0x54:
			case 0x55:
			case 0x56:
			case 0x57:
			case 0x58:
			case 0x59:
			case 0x5a:
			case 0x5f: // _ 
			case 0x61: // A-Z to 7a
			case 0x62:
			case 0x63:
			case 0x64:
			case 0x65:
			case 0x66:
			case 0x67:
			case 0x68:
			case 0x69:
			case 0x6a:
			case 0x6b:
			case 0x6c:
			case 0x6d:
			case 0x6e:
			case 0x6f:
			case 0x70:
			case 0x71:
			case 0x72:
			case 0x73:
			case 0x74:
			case 0x75:
			case 0x76:
			case 0x77:
			case 0x78:
			case 0x79:
			case 0x7a:
				result[length] = b;
				length++;
				break;
			default:
				int one = (int)b / 16;
				int two = (int)b - (one * 16);
				result[length] = 0x25;
				length++;
				if (one < 10)
					result[length] = (byte)(one + 48); // + 0x30;
				else
					result[length] = (byte)(one + 87); // + 0x61 - 10;
				length++;
				if (two < 10)
					result[length] = (byte)(two + 48); // + 0x30;
				else
					result[length] = (byte)(two + 87); // + 0x61 - 10;
				length++;
				break;
		}
		return new ArraySegment<byte>(result, 0, length);
	}

	public static string UrlEncode(string data)
	{
		byte[] utf8 = Encoding.UTF8.GetBytes(data);
		ArraySegment<byte> result = UrlEncode(utf8);
		return Encoding.GetEncoding("iso-8859-1").GetString(result.Array, 0, result.Count);
	}
}
