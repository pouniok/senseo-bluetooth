﻿using System;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.App;
using Android.Provider;
using System.Collections.Generic;

namespace SenseoBT
{
	public class LogAdapter : BaseAdapter<TableItem>
	{
		List<TableItem> items;
		Activity context;

		public LogAdapter(Activity context, List<TableItem> items) : base()
		{
			this.context = context;
			this.items = items;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override TableItem this[int position]
		{
			get { return items[position]; }
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];
			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.CustomView, null);
			view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Heading;
			view.FindViewById<TextView>(Resource.Id.Text2).Text = item.SubHeading;
			view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
			return view;
		}
	}
}
