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
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Utils;
using Android.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Android.Graphics;

namespace CSU_PORTABLE.Droid.UI
{
    class StudentFragment : Fragment
    {
        const string TAG = "StudentFragment";
        private TextView textViewInfo;
        LinearLayout layoutProgress;
        LinearLayout layoutSelectClassroom;
        LinearLayout layoutSelectTemperature;
        LinearLayout layoutSubmit;
        ImageView buttonNext;
        ImageView buttonBack;
        ImageView buttonSubmit;
        Button buttonDone;

        TextView textViewQuestion;
        TextView textViewQuestionDescription;
        TextView textViewProgressMessage;
        TextView textViewSelectedClass;

        TextView textViewTooHot;
        TextView textViewHot;
        TextView textViewNormal;
        TextView textViewCold;
        TextView textViewTooCold;
        List<RoomModel> classList = null;
        List<QuestionModel> questionList = null;
        RecyclerView mRecyclerView;

        string selectedClass;
        string selectedAnswer;

        int selectedQuestionId = -1;
        int selectedRoomId = -1;
        int selectedAnswerId = -1;
        int userId;

        public Color VeryCold = Color.Argb(0, 193, 235, 244);
        public Color Cold = Color.Argb(0, 148, 221, 242);
        public Color Normal = Color.Argb(0, 150, 197, 245);
        public Color Hot = Color.Argb(0, 210, 207, 235);
        public Color VeryHot = Color.Argb(0, 235, 230, 207);



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.student_dashboard, container, false);
            view.SetBackgroundColor(Utils.Utils.PrimaryColor);
            textViewInfo = view.FindViewById<TextView>(Resource.Id.textViewInfo);
            layoutProgress = view.FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutSelectClassroom = view.FindViewById<LinearLayout>(Resource.Id.layout_select_classroom);
            layoutSelectTemperature = view.FindViewById<LinearLayout>(Resource.Id.layout_select_temperature);
            layoutSubmit = view.FindViewById<LinearLayout>(Resource.Id.layout_submit);

            textViewQuestion = view.FindViewById<TextView>(Resource.Id.textViewQuestion);
            textViewQuestionDescription = view.FindViewById<TextView>(Resource.Id.textViewQuestionDescription);
            textViewProgressMessage = view.FindViewById<TextView>(Resource.Id.textView_progressMessage);
            textViewSelectedClass = view.FindViewById<TextView>(Resource.Id.textView_selectedClass);
            textViewTooHot = view.FindViewById<TextView>(Resource.Id.textView_tooHot);
            textViewHot = view.FindViewById<TextView>(Resource.Id.textView_hot);
            textViewNormal = view.FindViewById<TextView>(Resource.Id.textView_feelingNormal);
            textViewCold = view.FindViewById<TextView>(Resource.Id.textView_cold);
            textViewTooCold = view.FindViewById<TextView>(Resource.Id.textView_tooCold);
            buttonNext = view.FindViewById<ImageView>(Resource.Id.buttonNext);
            buttonBack = view.FindViewById<ImageView>(Resource.Id.buttonBack);
            buttonSubmit = view.FindViewById<ImageView>(Resource.Id.buttonSubmit);
            buttonDone = view.FindViewById<Button>(Resource.Id.buttonDone);

            textViewTooHot.Alpha = 0.5f;
            textViewHot.Alpha = 0.5f;
            textViewNormal.Alpha = 0.5f;
            textViewCold.Alpha = 0.5f;
            textViewTooCold.Alpha = 0.5f;




            textViewTooHot.Click += delegate
            {
                selectedAnswer = "Too Hot";
                selectedAnswerId = (int)textViewTooHot.Tag;

                textViewTooHot.Alpha = 1f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(Utils.Utils.VeryHot);
                view.SetBackgroundColor(Utils.Utils.VeryHot);
            };
            textViewHot.Click += delegate
            {
                selectedAnswer = "Hot";
                selectedAnswerId = (int)textViewHot.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 1.0f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(Utils.Utils.Hot);
                view.SetBackgroundColor(Utils.Utils.Hot);

            };
            textViewNormal.Click += delegate
            {
                selectedAnswer = "Feeling Normal";
                selectedAnswerId = (int)textViewNormal.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 1f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(Utils.Utils.Normal);
                view.SetBackgroundColor(Utils.Utils.Normal);
            };
            textViewCold.Click += delegate
            {
                selectedAnswer = "Cold";
                selectedAnswerId = (int)textViewCold.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 1f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(Utils.Utils.Cold);
                view.SetBackgroundColor(Utils.Utils.Cold);
            };
            textViewTooCold.Click += delegate
            {
                selectedAnswer = "Too Cold";
                selectedAnswerId = (int)textViewTooCold.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 1f;

                layoutSelectTemperature.SetBackgroundColor(Utils.Utils.VeryCold);
                view.SetBackgroundColor(Utils.Utils.VeryCold);
            };
            buttonNext.Click += delegate
            {
                Log.Debug(TAG, "Next button click");
                if (selectedClass != null)
                {
                    showLayoutSelectTemperature();
                    ShowTemperatureQuestion();
                }
                else
                {
                    Utils.Utils.ShowToast(this.Context, "Please select a Room");
                }
            };
            buttonBack.Click += delegate
            {
                Log.Debug(TAG, "Back button click");
                ResetFeedback(view);
                showLayoutSelectClass();
            };
            buttonSubmit.Click += delegate
            {
                Log.Debug(TAG, "Submit button click");
                if (selectedAnswer != null)
                {
                    showLayoutSubmit();
                    submitFeedback(userId);
                    ResetFeedback(view);
                }
                else
                {
                    Utils.Utils.ShowToast(this.Context, "Please select an option");
                }
            };
            buttonDone.Click += delegate
            {
                Log.Debug(TAG, "Done button click");
                selectedClass = null;
                selectedRoomId = -1;
                textViewSelectedClass.Text = "";
                selectedAnswer = null;
                showLayoutSelectClass();
            };

            showLayoutProgress("Loading...");
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            userId = PreferenceHandler.GetUserDetails().UserId;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this.Activity);
                if (isNetworkEnabled)
                {
                    getRoomList();

                }
                else
                {
                    Utils.Utils.ShowToast(this.Context, "Please enable your internet connection !");
                    showLayoutInfo();
                }
            }
            else
            {
                Utils.Utils.ShowToast(this.Context, "Invalid User Id. Please Login Again !");
                showLayoutInfo();
            }

            return view;
        }

        private void ResetFeedback(View view)
        {
            selectedClass = null;
            selectedRoomId = -1;
            selectedAnswer = null;
            selectedAnswerId = -1;
            textViewSelectedClass.Text = "";
            textViewTooHot.Alpha = 0.5f;
            textViewHot.Alpha = 0.5f;
            textViewNormal.Alpha = 0.5f;
            textViewCold.Alpha = 0.5f;
            textViewTooCold.Alpha = 0.5f;
            layoutSelectTemperature.SetBackgroundColor(Utils.Utils.PrimaryColor);
            view.SetBackgroundColor(Utils.Utils.PrimaryColor);
        }

        private async void submitFeedback(int userId)
        {
            FeedbackModel feedbackModel = new FeedbackModel();
            feedbackModel.QuestionId = selectedQuestionId;
            feedbackModel.RoomId = selectedRoomId;
            feedbackModel.AnswerId = selectedAnswerId;
            feedbackModel.FeedbackDesc = selectedAnswer;

            Log.Debug(TAG, "Login() " + feedbackModel.ToString());
            showLayoutSubmittingFeedback("Submitting feedback...");
            var response = await InvokeApi.Invoke(Constants.API_GIVE_FEEDBACK, JsonConvert.SerializeObject(feedbackModel), HttpMethod.Post, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                this.Activity.RunOnUiThread(() =>
                {
                    submitFeedbackResponse(response);
                });
            }
        }

        private async void submitFeedbackResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(strContent);
                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Submit Feedback Successful");
                    showLayoutSubmit();
                }
                else
                {
                    Log.Debug(TAG, "Submit Feedback Failed");
                    showLayoutSelectTemperature();
                    Utils.Utils.ShowToast(this.Context, "Failed to submit feedback, please try again!");
                }
            }
            else
            {
                Log.Debug(TAG, "Feedback Failed");
                showLayoutSelectTemperature();
                Utils.Utils.ShowToast(this.Context, "Failed to submit feedback, please try again!");
            }
        }

        private void ShowTemperatureQuestion()
        {
            if (questionList != null && questionList.Count > 0)
            {
                QuestionModel queModel = questionList[0];
                selectedQuestionId = queModel.QuestionId;
                textViewQuestion.Text = queModel.QuestionDesc;

                List<AnswerModel> ansList = queModel.Answers;
                if (ansList != null && ansList.Count > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        switch (i + 1)
                        {
                            case 1:
                                textViewTooCold.Text = ansList[i].AnswerDesc;
                                textViewTooCold.Tag = ansList[i].AnswerId;
                                break;
                            case 2:
                                textViewCold.Text = ansList[i].AnswerDesc;
                                textViewCold.Tag = ansList[i].AnswerId;
                                break;
                            case 3:
                                textViewNormal.Text = ansList[i].AnswerDesc;
                                textViewNormal.Tag = ansList[i].AnswerId;
                                break;
                            case 4:
                                textViewHot.Text = ansList[i].AnswerDesc;
                                textViewHot.Tag = ansList[i].AnswerId;
                                break;
                            case 5:
                                textViewTooHot.Text = ansList[i].AnswerDesc;
                                textViewTooHot.Tag = ansList[i].AnswerId;
                                break;

                        }
                    }
                }
            }
        }

        private void showLayoutProgress(string message)
        {
            textViewProgressMessage.Text = message;
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Visible;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Gone;
        }

        private void showLayoutSubmittingFeedback(string message)
        {
            textViewProgressMessage.Text = message;
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Visible;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Visible;
        }
        private void showLayoutSelectClass()
        {
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Visible;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Gone;
        }
        private void showLayoutSelectTemperature()
        {
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Visible;
            layoutSubmit.Visibility = ViewStates.Gone;
        }
        private void showLayoutSubmit()
        {
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Visible;
        }
        private void showLayoutInfo()
        {
            textViewInfo.Visibility = ViewStates.Visible;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Gone;
        }

        private async Task getRoomList()
        {
            Log.Debug(TAG, "getRoomList()");
            var response = await InvokeApi.Invoke(Constants.API_GET_ALL_ROOMS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                this.Activity.RunOnUiThread(() =>
                {
                    getRoomListResponse(response);
                });
            }
        }

        private async void getRoomListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                classList = array.ToObject<List<RoomModel>>();
                getQuestionList();
                //showClasses();
            }
            else if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Utils.Utils.RefreshToken(this.Context);
            }
            else
            {
                Log.Debug(TAG, "getRoomListResponse() Failed");
                Utils.Utils.ShowToast(this.Context, "getRoomListResponse() Failed");
                showLayoutInfo();
            }
        }

        private async void getQuestionList()
        {
            Log.Debug(TAG, "getQuestionList()");
            var response = await InvokeApi.Invoke(Constants.API_GET_QUESTION_ANSWERS, string.Empty, HttpMethod.Get, PreferenceHandler.GetToken());
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                this.Activity.RunOnUiThread(() =>
                {
                    getQuestionListResponse(response);
                });
            }
        }

        private async void getQuestionListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                questionList = array.ToObject<List<QuestionModel>>();
                showClasses();
            }
            else if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Utils.Utils.RefreshToken(this.Context);
                getQuestionList();
            }
            else
            {
                Log.Debug(TAG, "getQuestionListResponse() Failed");
                Utils.Utils.ShowToast(this.Context, "getQuestionListResponse() Failed");
                showLayoutInfo();
            }
        }

        private void showClasses()
        {
            if (classList != null && questionList != null)
            {

                // Plug in the linear layout manager:
                RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(this.Activity);
                mRecyclerView.SetLayoutManager(mLayoutManager);

                // Plug in my adapter:
                StudentDashboardAdapter mAdapter = new StudentDashboardAdapter(classList);
                mAdapter.ItemClick += OnItemClick;
                mRecyclerView.SetAdapter(mAdapter);
                showLayoutSelectClass();
            }
            else
            {
                showLayoutInfo();
            }
        }

        void OnItemClick(object sender, int position)
        {
            int num = position + 1;
            selectedClass = classList[position].RoomName;
            textViewSelectedClass.Text = selectedClass;
            selectedRoomId = classList[position].RoomId;
        }
    }
}