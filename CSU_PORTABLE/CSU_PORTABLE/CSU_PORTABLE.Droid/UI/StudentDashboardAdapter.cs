using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using CSU_PORTABLE.Models;
using Android.Support.V7.Widget;
using Java.Text;

namespace CSU_PORTABLE.Droid.UI
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
            //vh.textViewBuilding.Text = mClassModels[position].Building;
            //vh.textViewBrackerDetail.Text = mClassModels[position].Breaker_details;
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
            //public TextView textViewBuilding { get; set; }
            //public TextView textViewBrackerDetail { get; set; }

            public ClassViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                textViewClass = itemView.FindViewById<TextView>(Resource.Id.textViewClass);
                //textViewBuilding = itemView.FindViewById<TextView>(Resource.Id.textViewBuilding);
                //textViewBrackerDetail = itemView.FindViewById<TextView>(Resource.Id.textViewBrackerDetail);
                itemView.Click += (sender, e) => listener(base.Position);
            }
        }
    }
}