using System;
using System.Collections.Generic;
using System.Text;

namespace WpfAppApiClientNetCore.Helpers
{
	public class Person
	{
		public int id { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string gender { get; set; }
		public string status { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }
		public string Etag { get; set; }
	}

	public class Pagination
	{
		public int total { get; set; }
		public int pages { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
	}

	public class MetaData
	{
		public Pagination pagination { get; set; }

		public MetaData()
		{
			this.pagination = new Pagination();
		}
	}

	public class PagePerson
	{
		public int code { get; set; }
		public MetaData meta { get; set; }
		public List<Person> data { get; set; }

		public PagePerson()
		{
			this.meta = new MetaData();
			this.data = new List<Person>();
		}
	}

	public class RequestReturn
	{
		public CustomFunctions myaction { get; set; }
		public int code { get; set; }
		public string content { get; set; }
		public bool isSuccessful { get; set; }
		public string customString { get; set; }
		public CustomFunctions recentAction { get; set; }
	}

	public enum CustomFunctions
	{
		Add,
		Edit,
		Delete,
		Page,
		Search
	}

	public class WebRequest
	{
		public string BaseURI { get; set; }
		public string RequestURI { get; set; }
		public string BearerToken { get; set; }
		public MyHttpVerb HttpVerb { get; set; }
		public string Body { get; set; }
	}

	public class WebReturn
	{
		public string ResponseContent { get; set; }
		public int code { get; set; }
		public string Etag { get; set; }
	}

	public enum MyHttpVerb
	{
		GET,
		POST,
		PATCH,
		DELETE
	}

}
