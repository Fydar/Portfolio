﻿namespace Portfolio.Instance.Models
{
	public class ResourceImageViewModel
	{
		public string Image { get; set; }
		public string Size { get; set; }
		public bool Unblur { get; set; } = true;

		public ResourceImageViewModel(string image)
		{
			Image = image;
		}

		public ResourceImageViewModel(string image, string size)
		{
			Image = image;
			Size = size;
		}
	}
}
