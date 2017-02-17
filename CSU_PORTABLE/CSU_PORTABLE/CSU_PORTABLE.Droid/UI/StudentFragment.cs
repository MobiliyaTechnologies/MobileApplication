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
using RestSharp;
using CSU_PORTABLE.Utils;
using Android.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
        Button buttonNext;
        Button buttonBack;
        Button buttonSubmit;
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
        Toast toast;
        List<ClassRoomModel> classList = null;
        List<QuestionModel> questionList = null;
        RecyclerView mRecyclerView;

        string selectedClass;
        string selectedAnswer;

        int selectedQuestionId = -1;
        int selectedClassId = -1;
        int selectedAnswerId = -1;
        int userId;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.student_dashboard, container, false);

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
            buttonNext = view.FindViewById<Button>(Resource.Id.buttonNext);
            buttonBack = view.FindViewById<Button>(Resource.Id.buttonBack);
            buttonSubmit = view.FindViewById<Button>(Resource.Id.buttonSubmit);
            buttonDone = view.FindViewById<Button>(Resource.Id.buttonDone);

            textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
            textViewCold.SetTextColor(Android.Graphics.Color.Gray);
            textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
            textViewHot.SetTextColor(Android.Graphics.Color.Gray);
            textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

            textViewTooHot.Click += delegate
            {
                selectedAnswer = "Too Hot";
                selectedAnswerId = (int)textViewTooHot.Tag;
                textViewTooHot.SetTextColor(Android.Graphics.Color.White);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooCold.Typeface, Android.Graphics.TypefaceStyle.Normal);

            };

            textViewHot.Click += delegate
            {
                selectedAnswer = "Too Hot";
                selectedAnswerId = (int)textViewHot.Tag;
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.White);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooCold.Typeface, Android.Graphics.TypefaceStyle.Normal);

            };

            textViewNormal.Click += delegate
            {
                selectedAnswer = "Feeling Normal";
                selectedAnswerId = (int)textViewNormal.Tag;
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.White);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooCold.Typeface, Android.Graphics.TypefaceStyle.Normal);

            };
            textViewCold.Click += delegate
            {
                selectedAnswer = "Too Cold";
                selectedAnswerId = (int)textViewCold.Tag;
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.White);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewTooCold.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);

            };
            textViewTooCold.Click += delegate
            {
                selectedAnswer = "Too Cold";
                selectedAnswerId = (int)textViewTooCold.Tag;
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.White);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Bold);

            };

            buttonNext.Click += delegate
            {
                Log.Debug(TAG, "Next button click");
                if(selectedClass != null)
                {
                    showLayoutSelectTemperature();
                    showTemperatureQuestion();
                } else
                {
                    ShowToast("Please select a Classroom");
                }
            };
            buttonBack.Click += delegate
            {
                Log.Debug(TAG, "Back button click");
                selectedClass = null;
                selectedClassId = -1;
                textViewSelectedClass.Text = "";
                showLayoutSelectClass();
            };
            buttonSubmit.Click += delegate
            {
                Log.Debug(TAG, "Submit button click");
                if (selectedAnswer!= null)
                {
                    showLayoutSubmit();
                    submitFeedback(userId);
                }
                else
                {
                    ShowToast("Please select an option");
                }
            };
            buttonDone.Click += delegate
            {
                Log.Debug(TAG, "Done button click");
                selectedClass = null;
                selectedClassId = -1;
                textViewSelectedClass.Text = "";
                selectedAnswer = null;
                showLayoutSelectClass();
            };

            showLayoutProgress("Loading...");
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            var preferenceHandler = new PreferenceHandler();
            userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this.Activity);
                if (isNetworkEnabled)
                {
                    getClassList(userId);
                    getQuestionList(userId);
                }
                else
                {
                    ShowToast("Please enable your internet connection !");
                    showLayoutInfo();
                }
            }
            else
            {
                ShowToast("Invalid User Id. Please Login Again !");
                showLayoutInfo();
            }

            return view;
        }

        private void submitFeedback(int userId)
        {
            FeedbackModel feedbackModel = new FeedbackModel();
            feedbackModel.QuestionId = selectedQuestionId;
            feedbackModel.ClassId = selectedClassId;
            feedbackModel.AnswerId = selectedAnswerId;
            feedbackModel.FeedbackDesc = selectedAnswer;

            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "Login() " + feedbackModel.ToString());

            var request = new RestRequest(Constants.API_GIVE_FEEDBACK + "/" + userId, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(feedbackModel);
            
            showLayoutSubmittingFeedback("Submitting feedback...");

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    this.Activity.RunOnUiThread(() => {
                        submitFeedbackResponse((RestResponse)response);
                    });
                }
            });
        }

        private void submitFeedbackResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(restResponse.Content);

                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Feedback Successful");
                    showLayoutSubmit();
                }
                else
                {
                    Log.Debug(TAG, "Feedback Failed");
                    showLayoutSelectTemperature();
                    ShowToast("Failed to submit feedback, please try again!");
                }
            }
            else
            {
                Log.Debug(TAG, "Feedback Failed");
                showLayoutSelectTemperature();
                ShowToast("Failed to submit feedback, please try again!");
            }
        }

        private void showTemperatureQuestion()
        {
            if(questionList != null && questionList.Count > 0) {
                QuestionModel queModel = questionList[0];
                selectedQuestionId = queModel.QuestionId;
                textViewQuestion.Text = queModel.QuestionDesc;

                List<AnswerModel> ansList = queModel.Answers;
                if(ansList != null && ansList.Count > 0)
                {
                    for(int i=0; i<5; i++)
                    {
                        switch(i+1)
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

        private void getClassList(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getAlertList()");

            var request = new RestRequest(Constants.API_GET_CLASS_ROOMS + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    this.Activity.RunOnUiThread(() => {
                        getClassListResponse((RestResponse)response);
                    });
                }
            });
        }

        private void getClassListResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                JArray array = JArray.Parse(restResponse.Content);
                classList = array.ToObject<List<ClassRoomModel>>();

                showClasses();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                ShowToast("getAlertListResponse() Failed");
                showLayoutInfo();
            }
        }

        private void getQuestionList(int userId)
        {
            RestClient client = new RestClient(Constants.SERVER_BASE_URL);
            Log.Debug(TAG, "getAlertList()");

            var request = new RestRequest(Constants.API_GET_QUESTION_ANSWERS + "/" + userId, Method.GET);

            client.ExecuteAsync(request, response =>
            {
                Console.WriteLine(response);
                if (response.StatusCode != 0)
                {
                    Log.Debug(TAG, "async Response : " + response.ToString());
                    this.Activity.RunOnUiThread(() => {
                        getQuestionListResponse((RestResponse)response);
                    });
                }
            });
        }

        private void getQuestionListResponse(RestResponse restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());

                JArray array = JArray.Parse(restResponse.Content);
                questionList = array.ToObject<List<QuestionModel>>();

                showClasses();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                ShowToast("getAlertListResponse() Failed");
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
            selectedClass = classList[position].ClassDescription;
            textViewSelectedClass.Text = selectedClass;
            selectedClassId = classList[position].ClassId;
            ShowToast("You are in " + selectedClass);
        }

        private void ShowToast(string message)
        {
            if (toast != null)
            {
                toast.Cancel();
            }
            toast = Toast.MakeText(this.Activity, message, ToastLength.Short);
            toast.Show();
        }
    }
}