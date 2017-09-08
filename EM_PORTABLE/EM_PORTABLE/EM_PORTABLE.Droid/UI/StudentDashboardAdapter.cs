using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using EM_PORTABLE.Models;
using Android.Support.V7.Widget;
using Java.Text;

namespace EM_PORTABLE.Droid.UI
{
    class StudentDashboardAdapter : RecyclerView.Adapter
    {
        const string TAG = "StudentDashboardAdapter";
        public List<RoomModel> mRoomModels;
        public event EventHandler<int> ItemClick;

        public StudentDashboardAdapter(List<RoomModel> roomModels)
        {
            mRoomModels = roomModels;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.class_item, parent, false);
            ClassViewHolder vh = new ClassViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ClassViewHolder vh = holder as ClassViewHolder;
            vh.textViewClass.Text = mRoomModels[position].RoomName;
        }

        public override int ItemCount
        {
            get { return mRoomModels.Count(); }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)

                ItemClick(this, position);
        }

        //view holder class
        public class ClassViewHolder : RecyclerView.ViewHolder
        {
            public TextView textViewClass { get; set; }

            public ClassViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                textViewClass = itemView.FindViewById<TextView>(Resource.Id.textViewClass);
                itemView.Click += (sender, e) => listener(base.Position);
            }
        }
    }
}