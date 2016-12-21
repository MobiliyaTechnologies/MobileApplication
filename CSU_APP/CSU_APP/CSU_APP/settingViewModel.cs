using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CSU_APP
{
    public class cellModel
    {
        public string thumbnailUrl { get; set; }
        public string title { get; set; }
    }

    public class settingViewModel
    {
        public ObservableCollection<cellModel> cellData { get; } = new ObservableCollection<cellModel>();
        public void setDataForTableRow()
        {
            cellModel model = new cellModel();
            var result = JsonConvert.DeserializeObject<List<cellModel>>("[{\"title\":\"Reset Password\",\"thumbnailUrl\":\"\"},{\"title\":\"Logout\",\"thumbnailUrl\":\"\"}]");
            foreach (cellModel item in result)
            {
                cellData.Add(item);
            }
        }
    }
}
