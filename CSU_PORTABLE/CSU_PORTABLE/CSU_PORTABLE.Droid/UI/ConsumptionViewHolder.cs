using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace CSU_PORTABLE.Droid.UI
{
    internal class ConsumptionViewHolder : RecyclerView.ViewHolder
    {

        public TextView textViewID { get; set; }
        public TextView textViewName { get; set; }
        public TextView textViewConsumed { get; set; }
        public TextView textViewExpected { get; set; }
        public TextView textViewOverUsed { get; set; }
        public TextView textViewConsumedValue { get; set; }
        public TextView textViewExpectedValue { get; set; }
        public TextView textViewOverusedValue { get; set; }
        public ImageView imageViewConsumed { get; set; }
        public ImageView imageViewExpected { get; set; }
        public ImageView imageViewOveruse { get; set; }

        public ConsumptionViewHolder(View itemView, Action<int> listener) : base(itemView)
        {

            textViewID = itemView.FindViewById<TextView>(Resource.Id.textViewID);
            textViewName = itemView.FindViewById<TextView>(Resource.Id.textViewName);
            textViewConsumedValue = itemView.FindViewById<TextView>(Resource.Id.textViewConsumedValue);
            textViewExpectedValue = itemView.FindViewById<TextView>(Resource.Id.textViewExpectedValue);
            textViewOverusedValue = itemView.FindViewById<TextView>(Resource.Id.textViewOverusedValue);
            textViewOverUsed = itemView.FindViewById<TextView>(Resource.Id.textViewOverUsed);
            itemView.Click += (sender, e) => listener(base.Position);

        }
    }
}