using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CSU_PORTABLE.Models;
using Android.Support.V7.Widget;

namespace CSU_PORTABLE.Droid.UI
{
    public class ConsumptionListAdapter : RecyclerView.Adapter
    {
        const string TAG = "ConsumptionListAdapter";
        public List<ConsumptionModel> mConsumptionModels;
        Context mContext;
        public event EventHandler<int> ItemClick;

        public override int ItemCount
        {
            get
            {
                return mConsumptionModels.Count;
            }
        }

        public ConsumptionListAdapter(Context context, List<ConsumptionModel> consumptionModels)
        {
            mConsumptionModels = consumptionModels;
            mContext = context;
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ConsumptionViewHolder consumptionHolder = holder as ConsumptionViewHolder;
            consumptionHolder.textViewName.Text = mConsumptionModels[position].Name;
            consumptionHolder.textViewID.Text = Convert.ToString(mConsumptionModels[position].Id);
            consumptionHolder.textViewConsumedValue.Text = Convert.ToString(mConsumptionModels[position].Consumed);
            consumptionHolder.textViewExpectedValue.Text = Convert.ToString(mConsumptionModels[position].Expected);
            consumptionHolder.textViewOverusedValue.Text = Convert.ToString(mConsumptionModels[position].Overused);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                       Inflate(Resource.Layout.ConsumptionItemView, parent, false);
            return new ConsumptionViewHolder(itemView, OnClick);

        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }


    }
}