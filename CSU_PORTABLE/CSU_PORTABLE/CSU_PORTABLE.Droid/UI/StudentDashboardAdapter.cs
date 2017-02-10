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
        public List<ClassRoomModel> mClassModels;

        public StudentDashboardAdapter(List<ClassRoomModel> classModels)
        {
            mClassModels = classModels;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.student_dashboard_item, parent, false);
            ClassViewHolder vh = new ClassViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ClassViewHolder vh = holder as ClassViewHolder;
            vh.textViewClass.Text = mClassModels[position].ClassDescription;
            vh.textViewBuilding.Text = mClassModels[position].Building;
            vh.textViewBrackerDetail.Text = mClassModels[position].Breaker_details;
        }

        public override int ItemCount
        {
            get { return mClassModels.Count(); }
        }

        //view holder class
        public class ClassViewHolder : RecyclerView.ViewHolder
        {
            public TextView textViewClass { get; set; }
            public TextView textViewBuilding { get; set; }
            public TextView textViewBrackerDetail { get; set; }

            public ClassViewHolder(View itemView) : base(itemView)
            {
                textViewClass = itemView.FindViewById<TextView>(Resource.Id.textViewClass);
                textViewBuilding = itemView.FindViewById<TextView>(Resource.Id.textViewBuilding);
                textViewBrackerDetail = itemView.FindViewById<TextView>(Resource.Id.textViewBrackerDetail);
            }
        }
    }
}