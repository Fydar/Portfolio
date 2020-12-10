﻿using System.Collections.Generic;

namespace Portfolio.Instance.Services.PageMetaProvider
{
	public struct MetaItem
	{
		public string Name { get; set; }
		public Dictionary<string, string> Values { get; set; }

		public MetaItem(string name = null, string property = null, string content = null)
		{
			Name = name;
			Values = new Dictionary<string, string>();

			if (property != null)
			{
				Values.Add("property", property);
			}

			if (content != null)
			{
				Values.Add("content", content);
			}
		}
	}
}