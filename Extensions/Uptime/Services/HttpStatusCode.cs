// -----------------------------------------------------------------------------
//  <copyright file="HttpStatusCode.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System.ComponentModel;

	public enum HttpStatusCode
	{
		Continue = 100,

		[Description("Switching protocols")] SwitchingProtocols = 101,
		OK = 200,
		Created = 201,
		Accepted = 202,
		[Description("Non-authoritative information")] NonAuthoritativeInformation = 203,
		[Description("No content")] NoContent = 204,
		[Description("Reset content")] ResetContent = 205,
		[Description("Partial Content")] PartialContent = 206,
		Ambiguous = 300,
		[Description("Multiple choices")] MultipleChoices = 300,
		Moved = 301,
		[Description("Moved permanently")] MovedPermanently = 301,
		Found = 302,
		Redirect = 302,
		[Description("Redirect method")] RedirectMethod = 303,
		[Description("See other")] SeeOther = 303,
		[Description("Not modified")] NotModified = 304,
		[Description("Use proxy")] UseProxy = 305,
		Unused = 306,
		[Description("Redirect keep verb")] RedirectKeepVerb = 307,
		[Description("Temporary redirect")] TemporaryRedirect = 307,
		[Description("Bad request")] BadRequest = 400,
		Unauthorized = 401,
		[Description("Payment required")] PaymentRequired = 402,
		Forbidden = 403,
		NotFound = 404,
		[Description("Method not allowed")] MethodNotAllowed = 405,
		[Description("Not acceptable")] NotAcceptable = 406,
		[Description("Proxy authentication required")] ProxyAuthenticationRequired = 407,
		[Description("Request timeout")] RequestTimeout = 408,
		Conflict = 409,
		Gone = 410,
		[Description("Length required")] LengthRequired = 411,
		[Description("Precondition failed")] PreconditionFailed = 412,
		[Description("Not modified")] RequestEntityTooLarge = 413,
		[Description("Request uri too long")] RequestUriTooLong = 414,
		[Description("Unsupported media type")] UnsupportedMediaType = 415,
		[Description("Requested range not satisfiable")] RequestedRangeNotSatisfiable = 416,
		[Description("Expectation failed")] ExpectationFailed = 417,
		[Description("Upgrade failed")] UpgradeRequired = 426,
		[Description("Internal server error")] InternalServerError = 500,
		[Description("Not implemented")] NotImplemented = 501,
		[Description("Bad gateway")] BadGateway = 502,
		[Description("Service unavailable")] ServiceUnavailable = 503,
		[Description("Gateway timeout")] GatewayTimeout = 504,
		[Description("HTTP version not supported")] HttpVersionNotSupported = 505,
	}
}
