namespace Fydar.Dev.WebApp;

public class OpenGraphModel
{
	public OpenGraphTitle Title { get; set; } = new();
	public OpenGraphDescription Description { get; set; } = new();
	public OpenGraphCanonicalUrl CanonicalUrl { get; set; } = new();
	public IOpenGraphObject[] Properties { get; set; } = Array.Empty<IOpenGraphObject>();
}

public class OpenGraphTitle
{
	public string Html { get; set; } = string.Empty;
	public string OpenGraph { get; set; } = string.Empty;
	public string Twitter { get; set; } = string.Empty;

	public OpenGraphTitle()
	{
	}

	public OpenGraphTitle(string value)
	{
		Html = value;
		OpenGraph = value;
		Twitter = value;
	}

	public static implicit operator OpenGraphTitle(string value)
	{
		return new OpenGraphTitle(value);
	}
}

public class OpenGraphDescription
{
	public string Default { get; set; } = string.Empty;
	public string OpenGraph { get; set; } = string.Empty;
	public string Twitter { get; set; } = string.Empty;

	public OpenGraphDescription()
	{
	}

	public OpenGraphDescription(string value)
	{
		Default = value;
		OpenGraph = value;
		Twitter = value;
	}

	public static implicit operator OpenGraphDescription(string value)
	{
		return new OpenGraphDescription(value);
	}
}

public class OpenGraphCanonicalUrl
{
	public string Html { get; set; } = string.Empty;
	public string OpenGraph { get; set; } = string.Empty;
	public string Twitter { get; set; } = string.Empty;

	public OpenGraphCanonicalUrl()
	{
	}

	public OpenGraphCanonicalUrl(string value)
	{
		Html = value;
		OpenGraph = value;
		Twitter = value;
	}

	public static implicit operator OpenGraphCanonicalUrl(string value)
	{
		return new OpenGraphCanonicalUrl(value);
	}
}

public interface IOpenGraphObject
{

}

public class OpenGraphModelImage : IOpenGraphObject
{
	public OpenGraphImageUrl Url { get; set; } = new();
	public OpenGraphImageAlt Alt { get; set; } = new();
}


public class OpenGraphImageUrl
{
	public string OpenGraphUrl { get; set; } = string.Empty;
	public string OpenGraphSecureUrl { get; set; } = string.Empty;
	public string TwitterUrl { get; set; } = string.Empty;

	public OpenGraphImageUrl()
	{
	}

	public OpenGraphImageUrl(string value)
	{
		OpenGraphUrl = value;
		OpenGraphSecureUrl = value;
		TwitterUrl = value;
	}

	public static implicit operator OpenGraphImageUrl(string value)
	{
		return new OpenGraphImageUrl(value);
	}
}

public class OpenGraphImageAlt
{
	public string OpenGraph { get; set; } = string.Empty;
	public string Twitter { get; set; } = string.Empty;

	public OpenGraphImageAlt()
	{
	}

	public OpenGraphImageAlt(string value)
	{
		OpenGraph = value;
		Twitter = value;
	}

	public static implicit operator OpenGraphImageAlt(string value)
	{
		return new OpenGraphImageAlt(value);
	}
}
