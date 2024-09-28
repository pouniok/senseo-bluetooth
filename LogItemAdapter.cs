using Android.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace SenseoBT
{
	public class LogItemAdapter : BaseAdapter<Log>
	{
		List<Log> logs;
		ListView listView;
		private readonly Activity activity;

		public LogItemAdapter (Activity activity, ListView lv, List<Log> logs)
		{
			this.logs = logs;
			this.activity = activity;
			this.listView = lv;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Log this [int index] {
			get { return this.logs [index]; }
		}

		public override int Count {
			get { return this.logs.Count; }
		}

		public void Add(Log log) {
			this.logs.Add (log);
			this.NotifyDataSetChanged();
			listView.SmoothScrollToPosition (this.Count - 1);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView;

			if (view == null) {
				view = this.activity.LayoutInflater.Inflate (Android.Resource.Layout.ActivityListItem, null);
			}

			var log = this.logs [position];

			TextView text1 = view.FindViewById<TextView> (Android.Resource.Id.Text1);
			text1.Text = log.Text;

			ImageView imageView = view.FindViewById<ImageView> (Android.Resource.Id.Icon);
			if (imageView != null)
				imageView.SetImageResource (log.Image);

			return view;
		}
	}
}

